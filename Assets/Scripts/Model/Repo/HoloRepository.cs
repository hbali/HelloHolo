using Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    class HoloRepository : BaseRepository<BaseModel>, IHoloRepository
    {
        public override void DeleteModel<T>(string id)
        {
        }

        public override void DeleteModel(Type t, string id)
        {
        }

        public override void UpdateModel<T>(T entity)
        {
        }
    }
}
