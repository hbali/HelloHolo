using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Model.Repo
{
    public interface IRepository<Model>
    {
        IEnumerable<T> GetAllModelsOfType<T>() where T : Model;
        T GetModel<T>(string id) where T : Model;
        Model GetModel(Type t, string id);

        void AddModel<T>(Model model, string id);
        void AddModel(Model model, Type t, string id);

        void UpdateModel<T>(T entity);
        void DeleteModel<T>(string id);
        void DeleteModel(Type t, string id);

        IEnumerable<T> GetModels<T>(Func<T, bool> predicate) where T : Model;
    }
}