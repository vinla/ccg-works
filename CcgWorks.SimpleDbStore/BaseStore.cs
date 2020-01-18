using System;
using System.Collections.Generic;
using CcgWorks.Core;
using Amazon.SimpleDB;
using Model = Amazon.SimpleDB.Model;
using System.Threading.Tasks;

namespace CcgWorks.SimpleDbStore
{
    public abstract class BaseStore<TEntity> where TEntity : BaseObject
    {
        private readonly IAmazonSimpleDB _simpleDBClient;

        protected BaseStore(IAmazonSimpleDB simpleDBClient)
        {
            _simpleDBClient = simpleDBClient ?? throw new ArgumentNullException();
        }

        protected abstract string Domain { get; }

        public async Task Add(TEntity item) 
        {
            await SaveItem(item, false);
        }

        public async Task Delete(Guid id)
        {
            var deleteRequest = new Model.DeleteAttributesRequest
            {
                DomainName = Domain,
                ItemName = id.ToString()
            };

            await _simpleDBClient.DeleteAttributesAsync(deleteRequest);
        }

        protected async Task<IEnumerable<TEntity>> LoadItems(string query)
        {
            var items = new List<TEntity>();

            var selectRequest = new Model.SelectRequest
            {
                SelectExpression = query                 
            };

            var response = new Model.SelectResponse
            {
                NextToken = String.Empty
            };
            
            do
            {
                selectRequest.NextToken = response.NextToken;
                response = await _simpleDBClient.SelectAsync(selectRequest);
            
                foreach(var item in response.Items)
                    items.Add(FromAttributes(item.Name, item.Attributes));                    
                
            } 
            while(!String.IsNullOrEmpty(response.NextToken));

            return items;
        }

        public async Task<TEntity> GetSingle(Guid id)
        {
            var request = new Model.GetAttributesRequest
            {
                DomainName = Domain,
                ItemName = id.ToString()
            };

            var response = await _simpleDBClient.GetAttributesAsync(request);
            return FromAttributes(id.ToString(), response.Attributes);
        }

        public async Task UpdateOne(Guid id, Action<TEntity> updateAction)
        {
            var item = await GetSingle(id);
            updateAction(item);
            await SaveItem(item, true);
        }

        protected async Task SaveItem(TEntity item, bool isUpdate)
        {
            var request = new Model.PutAttributesRequest 
            {
                DomainName = Domain,
                ItemName = item.Id.ToString(),
                Attributes = ToAttributes(item, isUpdate)                
            };

            await _simpleDBClient.PutAttributesAsync(request);
        }

        protected abstract TEntity FromAttributes(string item, List<Model.Attribute> attributes);
        
        protected abstract List<Model.ReplaceableAttribute> ToAttributes(TEntity entity, bool isUpdate);
    }
}