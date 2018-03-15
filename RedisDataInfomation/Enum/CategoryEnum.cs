using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataInfomation.Enum
{
    /// <summary>
    /// 分類資料相關列舉
    /// </summary>
    public class CategoryEnum
    {
        /// <summary>
        /// 前台分類型態
        /// </summary>
        public enum CATE_TYPE : short
        {
            /// <summary>
            /// 一般
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 行銷頁面
            /// </summary>
            MarketingPage = 1,
            /// <summary>
            /// 後台分類
            /// </summary>
            BackEnd = 2,
            /// <summary>
            /// 行銷分類
            /// </summary>
            Marketing = 3,
            /// <summary>
            /// 任選分類
            /// </summary>
            AnyBuy = 4
        }

        /// <summary>
        /// 展開分類種類
        /// </summary>
        public enum HIGHLIGHT_TYPE : byte
        {
            /// <summary>
            /// 無
            /// </summary>
            None = 0,
            /// <summary>
            /// 熱銷排行
            /// </summary>
            HotSale = 1,
            /// <summary>
            /// 新品上市
            /// </summary>
            NewGood = 2,
            /// <summary>
            /// 折價券專區
            /// </summary>
            Coupon = 3,
            /// <summary>
            /// 館長推薦
            /// </summary>
            BossPromo = 4
        }

        /// <summary>
        /// Banner類型
        /// </summary>
        public enum CATE_BANNER_TYPE : short
        {
            /// <summary>
            /// 標頭
            /// </summary>
            Header = 1,
            /// <summary>
            /// 標尾
            /// </summary>
            Footer = 2,
            /// <summary>
            /// 3	推薦品牌
            /// </summary>
            PromoBrand = 3
        }
        /// <summary>
        /// 分類頁商品類型
        /// </summary>
        public enum STORECATEGROUP_TYPE
        {
            /// <summary>
            /// 館主要商品
            /// </summary>
            DeptMainProduct = 1,
            /// <summary>
            /// 館商品帶Logo
            /// </summary>
            DeptProductLogo = 2,
            /// <summary>
            /// 大分類商品
            /// </summary>
            LCateProduct = 3,
            /// <summary>
            /// 中小分類商品
            /// </summary>
            MSCateProduct = 4
        }
        /// <summary>
        /// 分類頁Banner類型
        /// </summary>
        public enum STORECATEBANNER_TYPE
        {
            /// <summary>
            /// 館主要Banner
            /// </summary>
            DeptMainBanner = 1,
            /// <summary>
            /// 瘦的Banner
            /// </summary>
            ThinBanner = 2,
            /// <summary>
            /// 中小分類Banner
            /// </summary>
            MSCateBanner = 3

        }

        /// <summary>
        /// 分類層級
        /// </summary>
        public enum CATE_LEVEL
        {
            /// <summary>
            /// 館
            /// </summary>
            Store = 1,
            /// <summary>
            /// 大分類
            /// </summary>
            LargeCate = 2,
            /// <summary>
            /// 中分類
            /// </summary>
            MiddleCate = 3,
            /// <summary>
            /// 小分類
            /// </summary>
            SmallCate = 4
        }
    }
}
