using ETMall.WS.Core.Module.Categories;
using ETMall.WS.DataClass.Response.Category.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataInfomation.Repository
{
    public class CategoryRepository
    {
        private CategoryModule _dateCategoryModule;
        private Dictionary<string, List<FrontCategoryInfo>> _allCateData;

        public CategoryRepository()
        {
            _dateCategoryModule = new CategoryModule();
            _allCateData = new Dictionary<string, List<FrontCategoryInfo>>();
        }
        

        public Dictionary<string, List<FrontCategoryInfo>> SetCategoryDicData(int parent = 0)
        {
           return  _allCateData = _dateCategoryModule.GetAllCategoryDictionary();
        }
    }
}
