﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Objects;
using System.Reflection;

using DisplayMonkey.Language;
using System.Text.RegularExpressions;


namespace DisplayMonkey.Models
{
    public partial class TopContent
    {
        [
            Display(ResourceType = typeof(Resources), Name = "Count"),
        ]
        public int Count { get; set; }

        [
            Display(ResourceType = typeof(Resources), Name = "Name"),
        ]
        public string Name { get; set; }
    }

    [
        MetadataType(typeof(Level.Annotations)),
    ]
    public partial class Level 
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int LevelId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType=typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Locations"),
            ]
            public virtual ICollection<Location> Locations { get; set; }
        }
    }

    [
        MetadataType(typeof(Location.Annotations))
    ]
    public partial class Location
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IDRequired"),
            ]
            public int LocationId { get; set; }

            [   Display(ResourceType = typeof(Resources), Name = "Level"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "LevelRequired"),
            ]
            public int LevelId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "TemperatureUnit"),
                StringLength(1, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string TemperatureUnit { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Latitude"),
            ]
            public Nullable<double> Latitude { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Longitude"),
            ]
            public Nullable<double> Longitude { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "DateFormat"),
            ]
            public string DateFormat { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "TimeFormat"),
            ]
            public string TimeFormat { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "TimeZone"),
            ]
            public string TimeZone { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Cultre"), // important spelling!!!
            ]
            public string Culture { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Level"),
            ]
            public virtual Level Level { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Area"),
            ]
            public virtual Location Area { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Displays"),
            ]
            public virtual ICollection<Display> Displays { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frames"),
            ]
            public virtual ICollection<Frame> Frames { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Sublocations"),
            ]
            public virtual ICollection<Location> Sublocations { get; set; }
        }

        [
            NotMapped,
        ]
        public IEnumerable<Location> SelfAndChildren
        {
            get { return this.SelfAndChildren(l => l.Sublocations); }
        }

        [
            NotMapped,
        ]
        public string TempUnitTranslated
        {
            get 
            {
                if (this.TemperatureUnit == null)
                    return null;
                else
                    return Resources.ResourceManager.GetString(this.TemperatureUnit); 
            }
        }

        [
            NotMapped,
        ]
        public string TempUnitOrDefault
        {
            get
            {
                return this.TempUnitTranslated ?? Resources.FromArea;
            }
        }

        [
            NotMapped,
        ]
        public string DateFmtOrDefault
        {
            get { return this.DateFormat ?? Resources.FromArea; }
        }

        [
            NotMapped,
        ]
        public string TimeFmtOrDefault
        {
            get { return this.TimeFormat ?? Resources.FromArea; }
        }

        [
            NotMapped,
        ]
        public string LatitudeOrDefault
        {
            get
            {
                if (this.Latitude.HasValue)
                    return this.Latitude.ToString();
                else
                    return Resources.FromArea;
            }
        }

        [
            NotMapped,
        ]
        public string LongitudeOrDefault
        {
            get
            {
                if (this.Longitude.HasValue)
                    return this.Longitude.ToString();
                else
                    return Resources.FromArea;
            }
        }

        [
            NotMapped,
        ]
        public string WoeidOrDefault
        {
            get
            {
                if (this.Woeid.HasValue)
                    return this.Woeid.ToString();
                else
                    return Resources.FromArea;
            }
        }

        [
            NotMapped,
        ]
        public string TimeZoneOrDefault
        {
            get
            {
                if (this.TimeZone != null)
                    return this.TimeZone;
                else
                    return Resources.FromArea;
            }
        }

        [
            NotMapped,
        ]
        public string CultureOrDefault
        {
            get
            {
                return Info.SupportedCultures
                    .Where(c => c.Name == this.Culture)
                    .Select(c => c.DisplayName)
                    .FirstOrDefault() ?? Resources.FromAreaOrServerDefault
                    ;
            }
        }
    }

    [
        MetadataType(typeof(Canvas.Annotations))
    ]
    public partial class Canvas
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int CanvasId { get; set; }
            
            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Height"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(1, Int32.MaxValue, 
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Height { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Width"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(1, Int32.MaxValue, 
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Width { get; set; }

            [
                StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
                Display(ResourceType = typeof(Resources), Name = "BackgroundColor"),
            ]
            public string BackgroundColor { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "BackgroundImage"),
            ]
            public Nullable<int> BackgroundImage { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Content"),
            ]
            public virtual Content Content { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "FullScreen"),
            ]
            public virtual FullScreen FullScreen { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Displays"),
            ]
            public virtual ICollection<Display> Displays { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Panels"),
            ]
            public virtual ICollection<Panel> Panels { get; set; }
        }
    }

    [
        MetadataType(typeof(Panel.Annotations))
    ]
    public partial class Panel
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IDRequired"),
            ]
            public int PanelId { get; set; }

            [   Display(ResourceType = typeof(Resources), Name = "Canvas"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "CanvasRequired"),
            ]
            public int CanvasId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Top"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),

            ]
            public int Top { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Left"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Left { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Height"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(1, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Height { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Width"),
                Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Range(1, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Width { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Canvas"),
            ]
            public virtual Canvas Canvas { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frames"),
            ]
            public virtual ICollection<Frame> Frames { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "FullScreens"),
            ]
            public virtual ICollection<FullScreen> FullScreens { get; set; }
        }

        [
            NotMapped,
        ]
        public bool IsFullscreen
        {
            get
            {
                return this.Canvas != null && this.Canvas.FullScreen != null
                    ? this.PanelId == this.Canvas.FullScreen.PanelId
                    : false;
            }
        }
    }

    [
        MetadataType(typeof(FullScreen.Annotations))
    ]
    public partial class FullScreen
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int PanelId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Canvas"),
            ]
            public int CanvasId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "MaxIdleInterval"),
                /*Required(ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),*/
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public Nullable<int> MaxIdleInterval { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Canvas"),
            ]
            public virtual Canvas Canvas { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Panel"),
            ]
            public virtual Panel Panel { get; set; }
        }
    }

    [
        MetadataType(typeof(Frame.Annotations))
    ]
    public partial class Frame
    {
        internal class Annotations
        {
            [
               Display(ResourceType = typeof(Resources), Name = "ID"), 
            ]
            public int FrameId { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Panel"),
            ]
            public int PanelId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Duration"),
                Range(1, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int Duration { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "BeginsOn"),
                DataType(DataType.DateTime,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "DateTimeExpected"),
                //DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true),
            ]
            public Nullable<System.DateTime> BeginsOn { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "EndsOn"),
                DataType(DataType.DateTime,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "DateTimeExpected"),
                //DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true),
            ]
            public Nullable<System.DateTime> EndsOn { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Sort"), 
            ]
            public Nullable<int> Sort { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "DateCreated"),
            ]
            public System.DateTime DateCreated { get; protected set; }

            //public byte[] Version { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Template"),
            ]
            public int TemplateId { get; set; }
            
            [
                Display(ResourceType = typeof(Resources), Name = "CacheInterval"),
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
            ]
            public int CacheInterval { get; set; }     
       
            /**** frames ****/
            
            [
               Display(ResourceType = typeof(Resources), Name = "Clock"), 
            ]
            public virtual Clock Clock { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Panel"), 
            ]
            public virtual Panel Panel { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Memo"), 
            ]
            public virtual Memo Memo { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "News"), 
            ]
            public virtual News News { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Picture"), 
            ]
            public virtual Picture Picture { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Report"), 
            ]
            public virtual Report Report { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Video"), 
            ]
            public virtual Video Video { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Weather"),
            ]
            public virtual Weather Weather { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Html"),
            ]
            public virtual Html Html { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Outlook"),
            ]
            public virtual Outlook Outlook { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Locations"), 
            ]
            public virtual ICollection<Location> Locations { get; set; }
        }
  
        /*public enum FrameTypes
        {
            Clock,
            Html,
            Memo,
            //News,
            Outlook,
            Picture,
            Report,
            Video,
            Weather,
            YouTube
        }*/

        public enum TimingOptions : int
        {
            TimingOption_Pending = 0,
            TimingOption_Current = 1,
            TimingOption_Expired = 2
        }

        [
            Display(ResourceType = typeof(Resources), Name = "FrameType"),
            NotMapped,
        ]
        public virtual FrameTypes? FrameType
        {
            get
            {
                return this.Template.FrameType;

                /*if (this.Clock != null) return FrameTypes.Clock;
                if (this.Html != null) return FrameTypes.Html;
                if (this.Memo != null) return FrameTypes.Memo;
                //if (this.News != null) return FrameTypes.News;
                if (this.Outlook != null) return FrameTypes.Outlook;
                if (this.Picture != null) return FrameTypes.Picture;
                if (this.Report != null) return FrameTypes.Report;
                if (this.Video != null) return FrameTypes.Video;
                if (this.Weather != null) return FrameTypes.Weather;
                if (this.Youtube != null) return FrameTypes.YouTube;
                return null;*/
            }

            set { }
        }

        [
            NotMapped,
        ]
        public string TranslatedType 
        { 
            get 
            { 
                return ((FrameTypes)this.FrameType).Translate(); 
            } 
        }

        public static Expression<Func<Frame, bool>> FilterByFrameType(FrameTypes? frameType)
        {
            if (frameType == null)
                return (f => true);
            else
                return (f => f.Template.FrameType == frameType.Value);
            
            /*switch (frameType)
            {
                case FrameTypes.Clock:
                    return (frame => frame.Clock != null);
                case FrameTypes.Html:
                    return (frame => frame.Html != null);
                case FrameTypes.Memo:
                    return (frame => frame.Memo != null);
                //case FrameTypes.News:
                //    return (frame => frame.News != null);
                case FrameTypes.Outlook:
                    return (frame => frame.Outlook != null);
                case FrameTypes.Picture:
                    return (frame => frame.Picture != null);
                case FrameTypes.Report:
                    return (frame => frame.Report != null);
                case FrameTypes.Video:
                    return (frame => frame.Video != null);
                case FrameTypes.Weather:
                    return (frame => frame.Weather != null);
                case FrameTypes.YouTube:
                    return (frame => frame.Youtube != null);
                default:
                    return (frame => true);
            }*/
        }

        [
            NotMapped,
        ]
        public bool ShowDuration
        {
            get { return /*this.Clock == null && this.Weather == null*/ true; }
        }

        [
            NotMapped,
        ]
        public string Icon
        {
            get { return Frame.IconFromFrameType(this.FrameType); }
        }

        public static string IconFromFrameType(FrameTypes? _frameType)
        {
            if (_frameType.HasValue) switch (_frameType.Value)
            {
                case FrameTypes.Clock:
                    return "~/images/clock.png";

                case FrameTypes.Html:    
                    return "~/images/html.png";

                //case FrameTypes.News:
                //    return "~/images/news.png";

                case FrameTypes.Memo:
                    return "~/images/memo.png";

                case FrameTypes.Outlook:
                    return "~/images/calendar.png";

                case FrameTypes.Picture:
                    return "~/images/image.png";

                case FrameTypes.Report:
                    return "~/images/ssrs.png";

                case FrameTypes.Video:    
                    return "~/images/video_thmb.png";

                case FrameTypes.Weather:
                    return "~/images/weather.png";

                case FrameTypes.YouTube:   
                    return "~/images/youtube.png";
            }

            return "~/images/unknown.png";
        }
    }

    public class FrameSelector : Frame
    {
        [
            Display(ResourceType = typeof(Resources), Name = "FrameType"),
            Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FrameTypeRequired"),
        ]
        public override FrameTypes? FrameType { get; set; }
    }

    public class LocationSelector : Frame
    {
        [  Display(ResourceType = typeof(Resources), Name = "ID"),
           Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IDRequired"),
        ]
        public int LocationId { get; set; }

        [
            Display(ResourceType = typeof(Resources), Name = "LocationName"),
            Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "LocationNameRequired"),
        ]
        public string LocationName { get; set; }
    }

    [
        MetadataType(typeof(Memo.Annotations))
    ]
    public partial class Memo
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Subject"),
            ]
            public string Subject { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Body"),
            ]
            public string Body { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "ShortBody"),
            NotMapped,
        ]
        public string ShortBody 
        {
            get
            {
                return this.Body.Length > 100 ? this.Body.Substring(0, 100) + "..." : this.Body;
            }
        }

        public void init()
        {
            if (Frame != null)
            {
            }
        }
    }

    [
        MetadataType(typeof(Html.Annotations))
    ]
    public partial class Html
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Content"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ContentRequired"),
            ]
            public string Content { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }
        }

        public void init()
        {
            if (Frame != null)
            {
            }
        }
    }

    [
        MetadataType(typeof(Content.Annotations))
    ]
    public partial class Content
    {
        internal class Annotations
        {
            [
               Display(ResourceType = typeof(Resources), Name = "ID"), 
            ]
            public int ContentId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "MediaSource"),
                DataType(DataType.Upload),
            ]
            public byte[] Data { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Type"), 
            ]
            public ContentTypes Type { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Canvas"), 
            ]
            public virtual ICollection<Canvas> Canvas { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Pictures"), 
            ]
            public virtual ICollection<Picture> Pictures { get; set; }

            [
               Display(ResourceType = typeof(Resources), Name = "Videos"), 
            ]
            public virtual ICollection<Video> Videos { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Size"),
        ]
        public Nullable<int> Size { get; set; }
    }

    public partial class ContentWithSize
    {
        public int ContentId { get; set; }

        [
            Display(ResourceType = typeof(Resources), Name = "Size"),
        ]
        public Nullable<int> Size { get; set; }

        [
            Display(ResourceType = typeof(Resources), Name = "Type"),
        ]
        public ContentTypes Type { get; set; }

        [
            Display(ResourceType = typeof(Resources), Name = "Name"),
        ]
        public string Name { get; set; }
    }

    [
        MetadataType(typeof(ReportServer.Annotations))
    ]
    public partial class ReportServer
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int ServerId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "BaseURL"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "BaseURLRequired"),
            ]
            public string BaseUrl { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "User"),
            ]
            public string User { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Password"),
            ]
            public byte[] Password { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Domain"),
            ]
            public string Domain { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Reports"),
            ]
            public virtual ICollection<Report> Reports { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Password"),
            DataType(DataType.Password),
            NotMapped,
        ]
        public string PasswordUnmasked
        {
            get
            {
                //if (this.Password == null)
                //    return null;
                //else
                //    return RsaUtil.Decrypt(this.Password);
                return _pwdMask;
            }

            set
            {
                _passwordSet = true;
                if (value == null)
                    this.Password = null;
                else if (value != _pwdMask)
                    this.Password = RsaUtil.Encrypt(value);
                else
                    _passwordSet = false;
            }
        }

        private const string _pwdMask = "****************";
        public bool _passwordSet = false;
    }

    [
        MetadataType(typeof(Report.Annotations))
    ]
    public partial class Report
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ReportPath"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ReportPathRequired"),
            ]
            public string Path { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "RenderMode"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RenderModeRequired"),
            ]
            public RenderModes Mode { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Server"),
            ]
            public int ServerId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ReportServer"),
            ]
            public virtual ReportServer ReportServer { get; set; }
        }

        public void init()
        {
            if (Frame != null)
            {
                Frame.CacheInterval = 5;
            }
        }
    }

    [
        MetadataType(typeof(Clock.Annotations))
    ]
    public partial class Clock
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ClockType"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ClockTypeRequired"),
            ]
            public ClockTypes Type { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ShowDate"),
            ]
            public bool ShowDate { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ShowTime"),
            ]
            public bool ShowTime { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "TimeZone"),
            ]
            public string TimeZone { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Label"),
            ]
            public string Label { get; set; }
        }

        public void init()
        {
            if (Frame != null)
            {
                Frame.Duration = 600;
            }
        }
    }

    [
        MetadataType(typeof(Weather.Annotations))
    ]
    public partial class Weather
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "WeatherType"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "WeatherTypeRequired"),
            ]
            public WeatherTypes Type { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }
        }

        public void init()
        {
            if (Frame != null)
            {
                Frame.Duration = 600;
                Frame.CacheInterval = 5;
            }
        }
    }

    [
        MetadataType(typeof(Picture.Annotations))
    ]
    public partial class Picture
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "RenderMode"),
            ]
            public RenderModes Mode { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "LinkedMedia"),
            ]
            public int ContentId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frame"),
            ]
            public virtual Frame Frame { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "MediaOptions"),
        ]
        public Nullable<int> SavedContentId { get; set; }

        public static string[] SupportedFormats = new string[] {
            "BMP", "GIF", "JPG", "JPEG", "PNG", "TIF", "TIFF"
        };

        public void init()
        {
            if (Frame != null)
            {
                Frame.CacheInterval = 60;
            }
        }
    }

    [
        MetadataType(typeof(Video.Annotations))
    ]
    public partial class Video
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "PlayMuted"),
            ]
            public bool PlayMuted { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "AutoLoop"),
            ]
            public bool AutoLoop { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "LinkedMedia"),
            ]
            public virtual ICollection<Content> Contents { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "MediaOptions"),
        ]
        public Nullable<int> SavedContentId { get; set; }

        public static string[] SupportedFormats = new string[] {
            "AVI", "MP4", "MPG", "MPEG", "OGG", "WEBM"
        };

        public void init()
        {
            if (Frame != null)
            {
                Frame.CacheInterval = 60;
            }

            PlayMuted = true;
            AutoLoop = true;
        }
    }

    [
        MetadataType(typeof(Youtube.Annotations))
    ]
    public partial class Youtube
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "YoutubeId"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "YoutubeIdRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string YoutubeId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Volume"),
                Range(0, 100,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "VolumeRangeError"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "VolumeRequired"),
            ]
            public int Volume { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "AutoLoop"),
            ]
            public bool AutoLoop { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "YTAspect"),
                //Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "YTAspectRequired"),
            ]
            public YTAspect Aspect { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "YTQuality"),
                //Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AspectRequired"),
            ]
            public YTQuality Quality { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "YTStart"),
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "YTStartRequired"),
            ]
            public int Start { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "YTRate"),
                //Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AspectRequired"),
            ]
            public YTRate Rate { get; set; }
        }

        public void init()
        {
            if (Frame != null)
            {
            }

            Aspect = YTAspect.YTAspect_Auto;
            Quality = YTQuality.YTQuality_Default;
            Rate = YTRate.YTRate_Normal;
            Start = 0;
            Volume = 0;
        }
    }

    [
        MetadataType(typeof(ExchangeAccount.Annotations))
    ]
    public partial class ExchangeAccount
    {
        public void init()
        {
            this.EwsVersion = OutlookEwsVersions.OutlookEwsVersion_Exchange2007_SP1;
            this.Url = "https://outlook.office365.com/EWS/Exchange.asmx";
        }

        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int AccountId { get; set; }
            
            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }
            
            [
                Display(ResourceType = typeof(Resources), Name = "Account"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AccountRequired"),
                RegularExpression(_emailMsk,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "EmailInvalid"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Account { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Password"),
            ]
            public byte[] Password { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "EwsVersion"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EwsVersionRequired"),
            ]
            public OutlookEwsVersions EwsVersion { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "OutlookUrl"),
                StringLength(250, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Url { get; set; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Password"),
            DataType(DataType.Password),
            NotMapped,
        ]
        public string PasswordUnmasked
        {
            get
            {
                return _pwdMask;
            }

            set
            {
                _passwordSet = true;
                if (value == null)
                    this.Password = null;
                else if (value != _pwdMask)
                    this.Password = RsaUtil.Encrypt(value);
                else
                    _passwordSet = false;
            }
        }

        private const string _pwdMask = "****************";
        public bool _passwordSet = false;
        public const string _emailMsk = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
    }

    [
        MetadataType(typeof(Outlook.Annotations))
    ]
    public partial class Outlook
    {
        public Outlook()
        {
            this.Mode = OutlookModes.OutlookMode_Today;
            this.ShowEvents = 0;
        }
        
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int FrameId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "View"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ViewRequired"),
            ]
            public OutlookModes Mode { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                //Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ExchangeAccount"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ExchangeAccountRequired"),
            ]
            public int AccountId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Mailbox"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MailboxRequired"),
                RegularExpression(ExchangeAccount._emailMsk, 
                    ErrorMessageResourceType = typeof(Resources), 
                    ErrorMessageResourceName = "EmailInvalid"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Mailbox { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ShowEvents"),
                Range(0, Int32.MaxValue,
                    ErrorMessageResourceType = typeof(Resources),
                    ErrorMessageResourceName = "PositiveIntegerRequired"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ShowEventsRequired"),
            ]
            public int ShowEvents { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ExchangeAccount"),
            ]
            public virtual ExchangeAccount ExchangeAccount { get; set; }
        }

        [
            NotMapped,
        ]
        public string NameOrMailboxOrAccount { get { return this.Name ?? this.Mailbox ?? this.ExchangeAccount.Name; } }

        public void init()
        {
            if (Frame != null)
            {
                Frame.CacheInterval = 1;
            }
        }
    }

    [
        MetadataType(typeof(Display.Annotations))
    ]
    public partial class Display
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "ID"),
            ]
            public int DisplayId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Canvas"),
            ]
            public int CanvasId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Location"),
            ]
            public int LocationId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }
            
            [
                Display(ResourceType = typeof(Resources), Name = "Host"),
            ]
            public string Host { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "ShowErrors"),
            ]
            public bool ShowErrors { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Canvas"),
            ]
            public virtual Canvas Canvas { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Location"),
            ]
            public virtual Location Location { get; set; }

        }
    }

    [
        MetadataType(typeof(Template.Annotations))
    ]
    public partial class Template
    {
        internal class Annotations
        {
            public int TemplateId { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Name"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NameRequired"),
                StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MaxLengthExceeded"),
            ]
            public string Name { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Content"),
                Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ContentRequired"),
            ]
            public string Html { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "FrameType"),
            ]
            public FrameTypes FrameType { get; set; }
            
            //public byte[] Version { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Frames"),
            ]
            public virtual ICollection<Frame> Frames { get; set; }
        }
    }

    [
        MetadataType(typeof(Setting.Annotations))
    ]
    public partial class Setting
    {
        internal class Annotations
        {
            [
                Display(ResourceType = typeof(Resources), Name = "Key"),
            ]
            public System.Guid Key { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Value"),
            ]
            public byte[] Value { get; set; }

            [
                Display(ResourceType = typeof(Resources), Name = "Type"),
            ]
            public SettingTypes Type { get; set; }
        }

        public static Guid Key_MaxImageSize { get; private set; }
        public static Guid Key_MaxVideoSize { get; private set; }
        public static Guid Key_PresentationSite { get; private set; }
        public static Guid Key_HelpSite { get; private set; }

        static Setting()
        {
            Key_MaxImageSize = new Guid("9A0BC012-FF01-4103-8A75-A03B275B0AD1");
            Key_MaxVideoSize = new Guid("4CAB57C4-EFEF-4EDE-91A3-EFFD48660909");
            Key_PresentationSite = new Guid("417D856B-7EC4-4CBD-A5EA-47BFC0F7B1F9");
            Key_HelpSite = new Guid("30F7521D-0058-4ED5-9B1E-D0AB0BACB7D8");
        }
        
        [
            Display(ResourceType = typeof(Resources), Name = "Setting"),
        ]
        public string Name
        {
            get
            {
                if (this.Key == Key_MaxImageSize)
                    return Resources.Settings_MaxImageSize;

                if (this.Key == Key_MaxVideoSize)
                    return Resources.Settings_MaxVideoSize;

                if (this.Key == Key_PresentationSite)
                    return Resources.Settings_PresentationSite;

                if (this.Key == Key_HelpSite)
                    return Resources.Settings_HelpSite;

                return this.Key.ToString();
            }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Value"),
            Required(ErrorMessageResourceType = typeof(Resources),
                ErrorMessageResourceName = "IntegerRequired"),
            DisplayFormat(ApplyFormatInEditMode = false,
                DataFormatString = "{0:0,###}"),
        ]
        public int IntValue
        {
            get { return this.Value == null ? 0 : BitConverter.ToInt32(this.Value.Reverse().ToArray(), 0); }
            set { this.Value = BitConverter.GetBytes(value).Reverse().ToArray(); }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Value"),
            Required(ErrorMessageResourceType = typeof(Resources),
                ErrorMessageResourceName = "PositiveIntegerRequired"),
            Range(0, Int32.MaxValue,
                ErrorMessageResourceType = typeof(Resources),
                ErrorMessageResourceName = "PositiveIntegerRequired"),
            DisplayFormat(ApplyFormatInEditMode = false, 
                DataFormatString = "{0:0,###}"),
        ]
        public int IntValuePositive
        {
            get { return this.IntValue; }
            set { this.IntValue = value; }
        }

        [
            Display(ResourceType = typeof(Resources), Name = "Value"),
        ]
        public string StringValue
        {
            get { return this.Value == null ? null : Encoding.ASCII.GetString(this.Value); }
            set { this.Value = value == null ? null : Encoding.ASCII.GetBytes(value); }
        }
    }






    #region Sundry


    public static class LinqExtension
    {
        public static IEnumerable<T> SelfAndChildren<T>(
            this T self,
            Func<T, IEnumerable<T>> children
            )
        {
            yield return self;
            IEnumerable<T> elements = children(self);
            foreach (T c in elements.SelectMany(e => e.SelfAndChildren(children)))
            {
                yield return c;
            }
        }
    }







    public partial class DisplayMonkeyEntities : DbContext
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage)
                    ;

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                //var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                //throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                throw new System.Data.Entity.Validation.DbEntityValidationException(fullErrorMessage, ex.EntityValidationErrors);
            }
        }

        /*static void ObjectContext_SavingChanges(object sender, EventArgs args)
        {
            ObjectContext context = sender as ObjectContext;
            if (context != null)
            {
                foreach (ObjectStateEntry e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    Type type = e.Entity.GetType();
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        var test = property.GetCustomAttributes();
                        
                        var metaDataList = property.GetCustomAttributes(typeof(MetadataTypeAttribute), false);
                        foreach (MetadataTypeAttribute metaData in metaDataList)
                        {
                            properties = metaData.MetadataClassType.GetProperties();
                            foreach (PropertyInfo subproperty in properties)
                            {
                                var attributes = subproperty.GetCustomAttributes(typeof(NeverUpdateAttribute), false);
                                if (attributes.Length > 0)
                                    e.RejectPropertyChanges(property.Name);
                            }
                        }
                    }
                }
            }
        }*/
    }

    //public class NeverUpdateAttribute : System.Attribute
    //{
    //}

    #endregion
}