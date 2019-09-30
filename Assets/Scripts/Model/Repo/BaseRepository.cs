using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Model.Repo
{
    /// <summary>
    /// Platform independent implementation of a basic repository pattern.
    /// </summary>
    /// <typeparam name="Model">The parent class of all models in the platform</typeparam>
    public abstract class BaseRepository<Model> : IRepository<Model>
    {
        protected Dictionary<Type, Dictionary<string, Model>> models;

        public BaseRepository()
        {
            Initialize();
        }

        /// <summary>
        /// Returns all of the specified type
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllModelsOfType<T>() where T : Model
        {
            return models[typeof(T)].Values.OfType<T>();
        }

        /// <summary>
        /// Gets models using a predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetModels<T>(Func<T, bool> predicate) where T : Model
        {
            return models[typeof(T)].Values.OfType<T>().Where(predicate);
        }



        public void AddModel<T>(Model model, string id)
        {
            AddModel(model, typeof(T), id);
        }

        public void AddModel(Model model, Type t, string id)
        {
            models[t][id] = model;
        }

        /// <summary>
        /// Gets T model with the specified id. Returns null if there is no model found
        /// </summary>
        /// <typeparam name="T">Type of model to return</typeparam>
        /// <param name="id">Key to search for</param>
        /// <returns></returns>
        public T GetModel<T>(string id) where T : Model
        {
            return (T)GetModel(typeof(T), id);
        }

        public Model GetModel(Type type, string id)
        {
            try
            {
                return models[type][id];
            }
            catch
            {
                return default(Model);
            }
        }

        /// <summary>
        /// Creates the dictionaries for all types that are the children of Model
        /// </summary>
        private void Initialize()
        {
            models = new Dictionary<Type, Dictionary<string, Model>>();
            foreach (Type t in /*Assembly.GetAssembly*/(typeof(Model)).GetTypeInfo().Assembly.GetTypes()/*.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Model)))*/)
            {
                if (!models.ContainsKey(t))
                {
                    models.Add(t, new Dictionary<string, Model>());
                }
            }
        }

        /// <summary>
        /// Updates the model in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity to update to</param>
        public abstract void UpdateModel<T>(T entity);

        /// <summary>
        /// Deletes the model from the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public abstract void DeleteModel<T>(string id);

        public abstract void DeleteModel(Type t, string id);
    }
}