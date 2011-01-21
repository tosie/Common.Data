using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using SubSonic.Repository;

namespace Common.Data {
    public class FormData : DbRecord, IDbRecord {

        #region Properties

        [SubSonicIgnore]
        public new Int64 Id { get; set; }

        [SubSonicPrimaryKey(false)]
        public String Name { get; set; }

        public Int32 Left { get; set; }
        public Int32 Top { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }
        public Int32 WindowState { get; set; }

        #endregion

        #region Custom Model Methods

        public void FillWithFormData(Form form) {
            WindowState = (Int32)form.WindowState;

            // Make sure the location and dimension don't represent a maximized or minimized state
            if (form.WindowState == FormWindowState.Normal) {
                Left = form.Left;
                Top = form.Top;
                Width = form.Width;
                Height = form.Height;
            } else {
                Left = form.RestoreBounds.Left;
                Top = form.RestoreBounds.Top;
                Width = form.RestoreBounds.Width;
                Height = form.RestoreBounds.Height;
            }
        }

        public static void EnsureVisibility(Form form) {
            Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

            foreach (Screen screen in Screen.AllScreens)
                rect = Rectangle.Union(rect, screen.WorkingArea);

            int max_screen_width = rect.Width; 
            int max_screen_height = rect.Height;

            // Move the window, if necessary
            if (form.Bottom > max_screen_height)
                form.Top = max_screen_height - form.Height;

            if (form.Right > max_screen_width)
                form.Left = max_screen_width - form.Width;
        }

        public static void LoadFormData(Form form) {
            FormData data = Read(form.Name);
            if (data == null)
                return;

            form.Left = data.Left;
            form.Top = data.Top;
            form.Width = data.Width;
            form.Height = data.Height;

            EnsureVisibility(form);

            form.WindowState = (FormWindowState)data.WindowState;
        }

        public static void SaveFormData(Form form) {
            FormData data = Read(form.Name);
            if (data == null) {
                data = Create(form);
            } else {
                data.FillWithFormData(form);
                data.Update();
            }
        }

        #endregion

        #region DbRecord Members

        public static SimpleRepository ActiveRepository;

        public static FormData Create(Object Tag) {
            var data = FormData.Create<FormData>(ActiveRepository, Tag);

            // For some reason the Name property's value is replaced by an auto-incrementing value ...
            if (Tag != null && Tag is Form) {
                data.Name = ((Form)Tag).Name;
                data.Update();
            }

            return data;
        }

        public static FormData Read(Int64 Id) {
            return FormData.Read<FormData>(ActiveRepository, Id);
        }

        public static FormData Read(String Name) {
            // Look inside the object cache if it has already been loaded
            List<FormData> Cache = GetCache<FormData>(ActiveRepository);
            FormData instance = Cache.Find(i => i.Name == Name);
            if (instance != null)
                return instance;

            // If not found in the cache, get it from the database
            try {
                instance = ActiveRepository.Find<FormData>(i => i.Name == Name).First();
                Trace.Assert(instance != null);
            } catch {
                return null;
            }

            // Let the object load itself
            instance.IsDeleted = false;
            instance.OwningRepository = ActiveRepository;
            instance.AfterLoad();

            // Add the instance to the cache
            Cache.Add(instance);

            // Return the object
            return instance;
        }

        public static List<FormData> Read() {
            return FormData.Read<FormData>(ActiveRepository);
        }

        public void Update() {
            Update<FormData>();
        }

        public bool Delete() {
            return Delete<FormData>();
        }

        #endregion

        #region IDbRecord Members

        public void InitializeWithDefaults(Object Tag) {
            if (Tag == null)
                throw new ArgumentNullException("Tag");

            Form form = (Form)Tag;
            Name = form.Name;
            FillWithFormData(form);
        }

        public void AfterLoad() {
            //
        }

        public bool BeforeUpdate() {
            return true;
        }

        public bool BeforeDelete() {
            return true;
        }

        #endregion

    }
}
