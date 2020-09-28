﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvORM.Connection.Classes.CommandBuilders
{
    class CommandInsertBuilderSqlServer
    {

        private readonly ModelEntity _modelEntity;

        public CommandInsertBuilderSqlServer(ModelEntity model)
        {
            _modelEntity = model;
        }

        internal string GetSqlInsert(out Dictionary<string, object> parametersValues)
        {
            var sql = new StringBuilder();

            sql.Append("INSERT INTO ");
            sql.Append(_modelEntity.TableName);

            GetSqlFieldsAndParameters(out var fields,
                                      out var values,
                                      out parametersValues);

            sql.Append(fields);

            sql.Append(" VALUES ");

            sql.Append(values);

            return sql.ToString();
        }



        private void GetSqlFieldsAndParameters(out string fields, out string values, out Dictionary<string, object> parametersValues)
        {
            parametersValues = new Dictionary<string, object>();

            var sqlFields = new StringBuilder();
            var sqlValues = new StringBuilder();

            sqlFields.Append(" (");
            sqlValues.Append(" (");
            foreach (var columnModelEntity in _modelEntity.ColumnsModelEntity)
            {
                sqlFields.Append(columnModelEntity.ColumnName);
                sqlFields.Append(",");

                var parameter = "@" + columnModelEntity.ColumnName;

                sqlValues.Append(parameter);
                sqlValues.Append(",");

                parametersValues.Add(parameter, columnModelEntity.Value);

            }

            sqlFields.Remove(sqlFields.Length - 1, 1);
            sqlFields.Append(") ");

            sqlValues.Remove(sqlValues.Length - 1, 1);
            sqlValues.Append(") ");

            fields = sqlFields.ToString();
            values = sqlValues.ToString();
        }
    }
}
