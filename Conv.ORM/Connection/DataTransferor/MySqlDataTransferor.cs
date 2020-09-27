﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConvORM.Connection.Classes;
using ConvORM.Connection.Classes.QueryBuilders;
using ConvORM.Connection.DataTransferor.Interfaces;
using ConvORM.Repository;

namespace ConvORM.Connection.DataTransferor
{
    internal class MySqlDataTransferor : IDataTransfer
    {
        private readonly ModelEntity _modelEntity;
        private readonly Connection _connection;

        public MySqlDataTransferor(ModelEntity modelEntity, Connection connection)
        {
            _modelEntity = modelEntity;
            _connection = connection;
        }

        public Entity Delete()
        {
            throw new System.NotImplementedException();
        }

        public Entity Insert()
        {
            var commandBuilder = new CommandBuilder(_modelEntity);

            if (_connection.ConnectionDriver()
                .ExecuteCommand(commandBuilder.GetSqlInsert(out var parametersValues), parametersValues) > 0)
            {
                var lastInsertedId = _connection.ConnectionDriver().GetLastInsertedId();
                var conditionsBuilder = new QueryConditionsBuilder();
                foreach (var column in _modelEntity.GetPrimaryFields())
                {

                    conditionsBuilder.AddQueryCondition(column.ColumnName, Enums.EConditionTypes.Equals,
                        new object[] {lastInsertedId});
                }

                return _connection.ConnectionDriver().ExecuteScalarQuery(commandBuilder.GetSqlSelect(conditionsBuilder),
                    _modelEntity.EntityType);
            }
            else
                return null;
        }

        public Entity SetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public Entity Update()
        {
            var commandBuilder = new CommandBuilder(_modelEntity);

            var conditionsBuilder = new QueryConditionsBuilder();
            foreach (var column in _modelEntity.GetPrimaryFields())
            {
                conditionsBuilder.AddQueryCondition(column.ColumnName, Enums.EConditionTypes.Equals,
                    new object[] { _modelEntity.GetPrimaryFieldValue(column.ColumnName) });
            }

            if (_connection.ConnectionDriver()
                .ExecuteCommand(commandBuilder.GetSqlUpdate(out var parametersValues, conditionsBuilder), parametersValues) > 0)
            {
                var lastInsertedId = _connection.ConnectionDriver().GetLastInsertedId();
                return Find(new int[] { lastInsertedId });
            }
            else
                return null;
        }

        public IList FindAll()
        {
            return _connection.ConnectionDriver()
                .ExecuteQuery(new CommandBuilder(_modelEntity).GetSqlSelect(new QueryConditionsBuilder()),
                    _modelEntity.EntityType);
        }

        public IList Find(QueryConditionsBuilder conditionsBuilder)
        {
            return _connection.ConnectionDriver()
                .ExecuteQuery(new CommandBuilder(_modelEntity).GetSqlSelect(conditionsBuilder),
                    _modelEntity.EntityType);
        }

        public Entity Find(int[] ids)
        {
            var conditionsBuilder = new QueryConditionsBuilder();
            int idsCount = 0;
            foreach (var column in _modelEntity.GetPrimaryFields())
            {
                conditionsBuilder.AddQueryCondition(column.ColumnName, Enums.EConditionTypes.Equals,
                    new object[] { ids[0] });

                idsCount++;
            }
            return _connection.ConnectionDriver()
                .ExecuteScalarQuery(new CommandBuilder(_modelEntity).GetSqlSelect(conditionsBuilder),
                    _modelEntity.EntityType);
        }
    }

}
