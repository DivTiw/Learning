using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{    
    public class MasterRepository<TEntity> : TTBaseRepository<TEntity> where TEntity : class
    {
        public MasterRepository(TTDBContext masterContext) : base(masterContext)
        {
        }
    }
}
