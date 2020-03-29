﻿using ConvORM.Connection.Classes;
using ConvORM.Connection.Enums;
using ConvORM.Repository;
using ConvORM.Repository.Entities;
using System;
using System.Collections.Generic;

namespace ConvORM.Connection.Helpers
{
    internal class ConverterModelEntityHelper
    {
        private readonly Entity _entity;
        private readonly Type _type;
        internal ConverterModelEntityHelper(Entity entity)
        {
            this._entity = entity;
            _type = this._entity.GetType();
        }

        internal string GetTableName()
        {
            var attributes = Attribute.GetCustomAttributes(_type);

            foreach(var attribute in attributes)
            {
                if (!(attribute is EntitiesAttributes)) continue;
                var entitiesAttributes = (EntitiesAttributes)attribute;
                return entitiesAttributes.TableName;
            }

            return _type.Name.Replace("Entity", "");
        }

        internal string GetConnectionName()
        {
            var attributes = Attribute.GetCustomAttributes(_type);

            foreach (var attribute in attributes)
            {
                if (!(attribute is EntitiesAttributes)) continue;
                var entitiesAttributes = (EntitiesAttributes)attribute;
                return entitiesAttributes.ConnectionName;
            }

            return "";
        }

        internal List<ColumnModelEntity> GetColumnsModelEntity()
        {
            var listColumnModelEntity = new List<ColumnModelEntity>();
            var fields = _type.GetFields();

            foreach(var field in fields)
            {
                var attributes = field.GetCustomAttributes(true);
                foreach(var attribute in attributes)
                {
                    if (!(attribute is EntitiesColumnAttributes entitiesColumnAttributes)) continue;
                    var columnModelEntity = new ColumnModelEntity
                    {
                        ColumnName = entitiesColumnAttributes.Name ?? field.Name,
                        Value = field.GetValue(_entity),
                        DataType = entitiesColumnAttributes.DataType == EDataTypes.None
                            ? ConvertSystemTypeToDataType(field.FieldType)
                            : entitiesColumnAttributes.DataType,
                        MaxSize = entitiesColumnAttributes.MaxSize,
                        Primary = entitiesColumnAttributes.Primary,
                        AutoGenerated = entitiesColumnAttributes.AutoGenerated,
                        Nullable = entitiesColumnAttributes.Nullable,
                        Relation = GetRelationColumnModelEntity(entitiesColumnAttributes.Relation)
                    };


                    listColumnModelEntity.Add(columnModelEntity);
                }

                if (attributes.Length != 0) continue;
                {
                    var columnModelEntity = new ColumnModelEntity
                    {
                        ColumnName = field.Name,
                        DataType = ConvertSystemTypeToDataType(field.FieldType),
                        Value = field.GetValue(_entity)
                    };
                    listColumnModelEntity.Add(columnModelEntity);
                }
            }

            return listColumnModelEntity;
        }

        private RelationColumnModelEntity GetRelationColumnModelEntity(EntitiesColumnRelationAttributes entitiesColumnRelationAttributes)
        {
            if (entitiesColumnRelationAttributes != null)
            {
                var relationColumnModelEntity = new RelationColumnModelEntity
                {
                    TableRelationName = entitiesColumnRelationAttributes.TableRelationName,
                    ColumnRelationName = entitiesColumnRelationAttributes.ColumnRelationName
                };
                return relationColumnModelEntity;
            }
            else
                return null;
        }

        private static EDataTypes ConvertSystemTypeToDataType(Type systemType)
        {
            switch (systemType.Name)
            {
                case "bool":
                    return EDataTypes.Boolean;
                case "decimal":
                    return EDataTypes.Decimal;
                case "double":
                    return EDataTypes.Boolean;
                case "float":
                    return EDataTypes.Float;
                case "int":
                    return EDataTypes.Integer;
                case "long":
                    return EDataTypes.Bigint;
                case "string":
                    return EDataTypes.Varchar;               
                default:
                    return EDataTypes.None;
            }
        }
    }
}
