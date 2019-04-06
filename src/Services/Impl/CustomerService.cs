using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class CustomerService : ICustomerService
    {
        private static IStorage dataStore;

        static CustomerService()
        {
            dataStore = new CosmosDbStorage(new CosmosDbStorageOptions()
            {
                AuthKey = Constants.CosmosDBKey,
                CollectionId = Constants.CosmosDBCustomersCollectionId,
                CosmosDBEndpoint = new Uri(Constants.CosmosServiceEndpoint),
                DatabaseId = Constants.CosmosDBDatabaseName,
            });
        }

        public async Task<CustomerState> GetCustomerStateById(string id)
        {
            string[] keys = { id };
            var customerStates = await dataStore.ReadAsync<CustomerState>(keys);
            if (customerStates.Count == 0)
            {
                var customerState = new CustomerState();
                customerState.Login = id;
                customerState.Cart = new CartState();
                customerState.Cart.Items = new List<PimItem>();
                var changes = new Dictionary<string, object>();
                {
                    changes.Add(id, customerState);
                }

                await dataStore.WriteAsync(changes);
                return customerState;
            }
            else
            {
                return customerStates.Values.First();
            }
        }

        public async Task UpdateCustomerState(CustomerState customerState)
        {
            var changes = new Dictionary<string, object>();
            {
                changes.Add(customerState.Login, customerState);
            }

            await dataStore.WriteAsync(changes);
        }
    }
}
