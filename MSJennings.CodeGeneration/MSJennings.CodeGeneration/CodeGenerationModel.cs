using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MSJennings.CodeGeneration
{
    public class CodeGenerationModel
    {
        private ModelNamespace _currentNamespace;
        private ModelEntity _currentEntity;
        private ModelProperty _currentProperty;

        public IList<ModelNamespace> Namespaces { get; } = new List<ModelNamespace>();

        public CodeGenerationModel SetCurrentNamespace(string name)
        {
            _currentNamespace = Namespaces.FirstOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal));

            if (_currentNamespace == null)
            {
                _currentNamespace = new ModelNamespace
                {
                    Model = this,
                    Name = name,
                };

                Namespaces.Add(_currentNamespace);
            }

            return this;
        }

        public CodeGenerationModel AddEntity(string name)
        {
            _currentEntity = new ModelEntity
            {
                Namespace = _currentNamespace,
                Name = name
            };

            _currentNamespace.Entities.Add(_currentEntity);

            return this;
        }

        public CodeGenerationModel AddProperty(string name, ModelPropertyLogicalType propertyType, bool isRequired = false)
        {
            _currentProperty = new ModelProperty
            {
                Entity = _currentEntity,
                Name = name,
                PropertyType = new ModelPropertyType
                {
                    LogicalType = propertyType,
                    ObjectTypeName = null,
                    ListItemType = null,
                },
                IsRequired = isRequired
            };

            _currentEntity.Properties.Add(_currentProperty);

            return this;
        }

        public CodeGenerationModel AddProperty(string name, string propertyTypeName, bool isRequired = false)
        {
            var property = new ModelProperty
            {
                Entity = _currentEntity,
                Name = name,
                PropertyType = new ModelPropertyType
                {
                    LogicalType = ModelPropertyLogicalType.Object,
                    ObjectTypeName = propertyTypeName,
                    ListItemType = null,
                },
                IsRequired = isRequired
            };

            _currentEntity.Properties.Add(property);

            return this;
        }

        public CodeGenerationModel AddListProperty(string name, ModelPropertyLogicalType listItemType, bool isRequired = false)
        {
            var property = new ModelProperty
            {
                Entity = _currentEntity,
                Name = name,
                PropertyType = new ModelPropertyType
                {
                    LogicalType = ModelPropertyLogicalType.List,
                    ObjectTypeName = null,
                    ListItemType = new ModelPropertyType
                    {
                        LogicalType = listItemType,
                        ObjectTypeName = null,
                        ListItemType = null,
                    },
                },
                IsRequired = isRequired
            };

            _currentEntity.Properties.Add(property);

            return this;
        }

        public CodeGenerationModel AddListProperty(string name, string listItemTypeName, bool isRequired = false)
        {
            var property = new ModelProperty
            {
                Entity = _currentEntity,
                Name = name,
                PropertyType = new ModelPropertyType
                {
                    LogicalType = ModelPropertyLogicalType.List,
                    ObjectTypeName = null,
                    ListItemType = new ModelPropertyType
                    {
                        LogicalType = ModelPropertyLogicalType.Object,
                        ObjectTypeName = listItemTypeName,
                        ListItemType = null
                    },
                },
                IsRequired = isRequired
            };

            _currentEntity.Properties.Add(property);

            return this;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new[] { new StringEnumConverter() },
            });
        }

        public void Reset()
        {
            Namespaces.Clear();
            _currentNamespace = null;
            _currentEntity = null;
            _currentProperty = null;
        }

        public void LoadFromOtherModel(CodeGenerationModel otherModel)
        {
            if (otherModel == null)
            {
                throw new ArgumentNullException(nameof(otherModel));
            }

            Reset();
            Namespaces.AddRange(otherModel.Namespaces);
        }

        public void LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            var deserializedModel = JsonConvert.DeserializeObject<CodeGenerationModel>(json);
            LoadFromOtherModel(deserializedModel);
        }

        public void LoadFromTypes(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            Reset();

            foreach (var type in types)
            {
                _ = SetCurrentNamespace(type.Namespace);
                _ = AddEntity(type.Name);

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var modelPropertyType = property.PropertyType.ToModelPropertyType();

                    if (modelPropertyType.LogicalType == ModelPropertyLogicalType.List)
                    {
                        _ = AddListProperty(property.Name, modelPropertyType.ListItemType.LogicalType, property.IsRequired());
                    }
                    else if (modelPropertyType.LogicalType == ModelPropertyLogicalType.Object)
                    {
                        _ = AddProperty(property.Name, property.PropertyType.Name, property.IsRequired());
                    }
                    else
                    {
                        _ = AddProperty(property.Name, modelPropertyType.LogicalType, property.IsRequired());
                    }
                }
            }
        }

        public void LoadFromAssembly(string assemblyFileName)
        {
            // todo: refactor this

            if (string.IsNullOrWhiteSpace(assemblyFileName))
            {
                throw new ArgumentNullException(nameof(assemblyFileName));
            }

            if (!File.Exists(assemblyFileName))
            {
                throw new FileNotFoundException("The file was not found at the specified path.", assemblyFileName);
            }

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFileName);

            foreach (var typeDefinition in assemblyDefinition.MainModule.Types)
            {
                if (typeDefinition.CustomAttributes.Any(x => x.AttributeType.Name.Equals(nameof(GeneratedCodeAttribute), StringComparison.Ordinal)))
                {
                    continue;
                }

                if (!typeDefinition.IsPublic)
                {
                    continue;
                }

                if (!typeDefinition.Properties.Any(x => x.PropertyType.Resolve().IsPublic))
                {
                    continue;
                }

                _ = SetCurrentNamespace(typeDefinition.Namespace);
                _ = AddEntity(typeDefinition.Name);

                foreach (var propertyDefinition in typeDefinition.Properties)
                {
                    if (!propertyDefinition.PropertyType.Resolve().IsPublic)
                    {
                        continue;
                    }

                    if (!propertyDefinition.HasThis) // does `HasThis` indicate an instance property?
                    {
                        continue;
                    }
                }
            }
        }

        private static string GetGenericTypeNameString(TypeReference typeReference)
        {
            var typeName = typeReference.Name;

            if (!typeReference.IsGenericInstance)
            {
                return typeName;
            }

            var genericTypeNameMarkerIndex = typeName.IndexOf('`', StringComparison.Ordinal);

            var genericTypeName =
                genericTypeNameMarkerIndex >= 0
                ? typeName.Substring(0, genericTypeNameMarkerIndex)
                : typeName;

            var genericInstanceType = (GenericInstanceType)typeReference;
            var genericArguments = genericInstanceType.GenericArguments;

            var genericArgumentsString = string.Join(",", genericArguments.Select(x => GetGenericTypeNameString(x)));
            return $"{genericTypeName}<{genericArgumentsString}>";
        }

        private static ModelPropertyType GetGenericModelPropertyType(TypeReference typeReference)
        {
            if (!typeReference.IsGenericInstance)
            {
                return typeReference.ToModelPropertyType();
            }

            var genericTypeNameMarkerIndex = typeName.IndexOf('`', StringComparison.Ordinal);

            var genericTypeName =
                genericTypeNameMarkerIndex >= 0
                ? typeName.Substring(0, genericTypeNameMarkerIndex)
                : typeName;

            var genericInstanceType = (GenericInstanceType)typeReference;
            var genericArguments = genericInstanceType.GenericArguments;

            var genericArgumentsString = string.Join(",", genericArguments.Select(x => GetGenericTypeNameString(x)));
            return $"{genericTypeName}<{genericArgumentsString}>";
        }
    }
}
