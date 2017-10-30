
using System;

namespace TimeLine
{
    //轨道item的属性
    [AttributeUsage(AttributeTargets.Class)]
    public class TimelineItemAttribute : Attribute
    {
        private string subCategory; // item子类型策略
        private string label; // item名称
        private TrackItemGenre[] genres; // item的类型

        // 特定的操作对象的类型
        private Type requiredObjectType; 

        /// <summary>
        /// Item attribute.
        /// </summary>
        /// <param name="category">The user friendly name of the category this cutscene item belongs to.</param>
        /// <param name="label">The user friendly name of the cutscene item.</param>
        /// <param name="genres">The genres that this Cutscene Item belongs to.</param>
        public TimelineItemAttribute(string category, string label, params TrackItemGenre[] genres)
        {
            this.subCategory = category;
            this.label = label;
            this.genres = genres;
        }

        /// <summary>
        /// The Cutscene Item attribute.
        /// </summary>
        /// <param name="category">The user friendly name of the category this cutscene item belongs to.</param>
        /// <param name="label">The user friendly name of the cutscene item.</param>
        /// <param name="pairedObject">Optional: required object to be paired with cutscene item.</param>
        /// <param name="genres">The genres that this Cutscene Item belongs to.</param>
        public TimelineItemAttribute(string category, string label, Type pairedObject, params TrackItemGenre[] genres)
        {
            this.subCategory = category;
            this.label = label;
            this.requiredObjectType = pairedObject;
            this.genres = genres;
        }

        /// <summary>
        /// 子策略
        /// </summary>
        public string Category
        {
            get
            {
                return subCategory;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// 轨道item的类型
        /// </summary>
        public TrackItemGenre[] Genres
        {
            get
            {
                return genres;
            }
        }

        /// <summary>
        /// 得到特定的对象类型 比如 音频轨道项目 需要audioclip对象
        /// </summary>
        public Type RequiredObjectType
        {
            get
            {
                return requiredObjectType;
            }
        }
    }
}