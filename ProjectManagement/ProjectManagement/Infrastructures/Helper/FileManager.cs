using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace ProjectManagement.Infrastructures.Helper
{
    public class FileManager
    {
        /// <summary>
        /// User this method for uploading file
        /// </summary>
        /// <param name="projectId">Current Project's ID</param>
        /// <param name="userDirectory">Directory Name of User type</param>
        /// <param name="moduleDirectory">Directory of Specific Actions</param>
        /// <param name="file">File</param>
        /// <returns></returns>
        public string Upload(long projectId, string userDirectory, string moduleDirectory, HttpPostedFileBase file)
        {
            if (projectId > 0 || (projectId == 0 && userDirectory == "postProduction"))
            {
                try
                {
                    const string initialPath = @"~/Content/UploadImage";
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        if (fileName != null)
                        {
                            var todayTime = DateTime.Now;
                            var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                                todayTime.Minute, todayTime.Second, todayTime.Millisecond);
                            var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff}", time);
                            fileName = timeFormat + "-" + fileName;
                            var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                            file.SaveAs(path);
                            string finalPath = @"C:\uploads" + @"\" + projectId + @"\" + userDirectory + @"\" + moduleDirectory;
                            if (Directory.Exists(finalPath))
                            {
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                // File.Delete(path);
                            }
                            else
                            {
                                Directory.CreateDirectory(finalPath);
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                // File.Delete(path);
                            }
                            return finalPath + "\\" + fileName;
                        }
                    }
                    return "failed";
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    return "failed";
                }
            }

            return "failed";
        }

        public string IncidentUpload(string userDirectory, string moduleDirectory, HttpPostedFileBase file)
        {
            try
            {
                const string initialPath = @"~/Content/UploadImage";
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (fileName != null)
                    {
                        var todayTime = DateTime.Now;
                        var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                            todayTime.Minute, todayTime.Second, todayTime.Millisecond);
                        var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff}", time);
                        fileName = timeFormat + "-" + fileName;
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                        file.SaveAs(path);
                        string finalPath = @"E:\uploads" + @"\" + userDirectory + @"\" + moduleDirectory;
                        if (Directory.Exists(finalPath))
                        {
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            // File.Delete(path);
                        }
                        else
                        {
                            Directory.CreateDirectory(finalPath);
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            //  File.Delete(path);
                        }
                        return finalPath + "\\" + fileName;
                    }
                }
                return "failed";
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "failed";
            }
        }

        public string DocManagementUpload(string userDirectory, string moduleDirectory, HttpPostedFileBase file)
        {

            try
            {
                const string initialPath = @"~/Content/UploadImage";
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (fileName != null)
                    {
                        var todayTime = DateTime.Now;
                        var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                            todayTime.Minute, todayTime.Second, todayTime.Millisecond);
                        //var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff}", time);
                        //fileName = timeFormat + "-" + fileName;
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                        file.SaveAs(path);
                        string finalPath = @"E:\uploads" + @"\" + userDirectory + @"\" + moduleDirectory;
                        if (Directory.Exists(finalPath))
                        {
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            // File.Delete(path);
                        }
                        else
                        {
                            Directory.CreateDirectory(finalPath);
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            // File.Delete(path);
                        }
                        return finalPath + "\\" + fileName;
                    }
                }
                return "failed";
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "failed";
            }
        }

        public string ImportExcelData(string userDirectory, string moduleDirectory, HttpPostedFileBase file)
        {
            try
            {
                const string initialPath = @"~/Content/UploadImage";
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (fileName != null)
                    {
                        var todayTime = DateTime.Now;
                        var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                            todayTime.Minute, todayTime.Second, todayTime.Millisecond);
                        var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff}", time);
                        fileName = timeFormat + "-" + fileName;
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                        file.SaveAs(path);
                        string finalPath = @"C:\uploads" + @"\" + userDirectory + @"\" + moduleDirectory;
                        if (Directory.Exists(finalPath))
                        {
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            //File.Delete(path);
                        }
                        else
                        {
                            Directory.CreateDirectory(finalPath);
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            //File.Delete(path);
                        }
                        return finalPath + "\\" + fileName;
                    }
                }
                return "failed";
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "failed";
            }
        }

        public string UploadPiClosing(string userDirectory, string moduleDirectory, HttpPostedFileBase file)
        {
            try
            {
                const string initialPath = @"~/Content/UploadImage";
                var fileSavePath = string.Empty;
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (fileName != null)
                    {
                        var todayTime = DateTime.Now;
                        var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                            todayTime.Minute, todayTime.Second, todayTime.Millisecond);
                        var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss-fff}", time);
                        fileName = timeFormat + "-" + fileName;
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                        file.SaveAs(path);
                        string tempPath = "";
                        string finalPath = @"C:\uploads" + @"\" + userDirectory + @"\" + moduleDirectory;
                        //if (Directory.Exists(finalPath))
                        //{
                        //    File.Copy(path, finalPath + "\\" + fileName, true);
                        //}
                        //else
                        //{
                        //    Directory.CreateDirectory(finalPath);
                        //    File.Copy(path, finalPath + "\\" + fileName, true);
                        //}
                        //return finalPath + "\\" + fileName;
                        if (Directory.Exists(finalPath))
                        {
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            tempPath = finalPath + "\\" + fileName;
                            // File.Delete(path);
                        }
                        else
                        {
                            Directory.CreateDirectory(finalPath);
                            File.Copy(path, finalPath + "\\" + fileName, true);
                            tempPath = finalPath + "\\" + fileName;
                            // File.Delete(path);
                        }

                        if (fileSavePath != "")
                        {
                            fileSavePath += "|" + tempPath;
                        }
                        else
                        {
                            fileSavePath = tempPath;
                        }
                    }
                }
                return fileSavePath;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return msg;
            }
        }
        public string Upload1(long projectId, string userDirectory, string moduleDirectory, List<HttpPostedFileBase> file)
        {
            if (projectId > 0)
            {
                try
                {
                    const string initialPath = @"~/Content/UploadImage";
                    // var fileName = "";
                    var fileSavePath = string.Empty;
                    int numFiles = file.Count;
                    //int uploadedCount = 0;
                    for (int i = 0; i < numFiles; i++)
                    {
                        var uploadedFile = file[i];
                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {


                            var fileName = Path.GetFileName(uploadedFile.FileName);
                            if (fileName != null)
                            {
                                var todayTime = DateTime.Now;
                                var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                                    todayTime.Minute, todayTime.Second);
                                var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss}", time);
                                fileName = timeFormat + "-" + fileName;
                                var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                                uploadedFile.SaveAs(path);
                                string tempPath = "";
                                string finalPath = @"C:\uploads" + @"\" + projectId + @"\" + userDirectory + @"\" +
                                                   moduleDirectory;

                                if (Directory.Exists(finalPath))
                                {
                                    File.Copy(path, finalPath + "\\" + fileName, true);
                                    tempPath = finalPath + "\\" + fileName;
                                    // File.Delete(path);
                                }
                                else
                                {
                                    Directory.CreateDirectory(finalPath);
                                    File.Copy(path, finalPath + "\\" + fileName, true);
                                    tempPath = finalPath + "\\" + fileName;
                                    //  File.Delete(path);
                                }

                                if (fileSavePath != "")
                                {
                                    fileSavePath += "|" + tempPath;
                                }
                                else
                                {
                                    fileSavePath = tempPath;
                                }
                            }

                        }

                    }
                    return fileSavePath;
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    return "failed";
                }
            }

            return "failed";
        }

        public string Upload3(long projectId, long swQcHeadId, long swQcAssignId, string userDirectory, string moduleDirectory, List<HttpPostedFileBase> file)
        {
            if (projectId > 0)
            {
                try
                {
                    const string initialPath = @"~/Content/UploadImage";
                    // var fileName = "";
                    var fileSavePath = string.Empty;
                    int numFiles = file.Count;
                    //int uploadedCount = 0;
                    for (int i = 0; i < numFiles; i++)
                    {
                        var uploadedFile = file[i];
                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {


                            var fileName = Path.GetFileName(uploadedFile.FileName);
                            if (fileName != null)
                            {
                                var todayTime = DateTime.Now;
                                var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                                    todayTime.Minute, todayTime.Second);
                                var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss}", time);
                                fileName = timeFormat + "-" + fileName;
                                var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                                uploadedFile.SaveAs(path);
                                string tempPath = "";
                                string finalPath = @"C:\uploads" + @"\" + projectId + "_" + swQcHeadId + "_" + swQcAssignId + @"\" + userDirectory + @"\" +
                                                   moduleDirectory;

                                if (Directory.Exists(finalPath))
                                {
                                    File.Copy(path, finalPath + "\\" + fileName, true);
                                    tempPath = finalPath + "\\" + fileName;
                                    // File.Delete(path);
                                }
                                else
                                {
                                    Directory.CreateDirectory(finalPath);
                                    File.Copy(path, finalPath + "\\" + fileName, true);
                                    tempPath = finalPath + "\\" + fileName;
                                    //  File.Delete(path);
                                }

                                if (fileSavePath != "")
                                {
                                    fileSavePath += "|" + tempPath;
                                }
                                else
                                {
                                    fileSavePath = tempPath;
                                }
                            }

                        }

                    }
                    return fileSavePath;
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    return "failed";
                }
            }

            return "failed";
        }

        public string UploadAnotherDrive(long projectId, string userDirectory, string moduleDirectory, List<HttpPostedFileBase> file)
        {
            // if (projectId > 0)
            // {
            try
            {
                const string initialPath = @"~/Content/UploadImage";
                // var fileName = "";
                var fileSavePath = string.Empty;
                int numFiles = file.Count;
                //int uploadedCount = 0;
                for (int i = 0; i < numFiles; i++)
                {
                    var uploadedFile = file[i];
                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(uploadedFile.FileName);
                        if (fileName != null)
                        {
                            var todayTime = DateTime.Now;
                            var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                                todayTime.Minute, todayTime.Second);
                            var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss}", time);
                            fileName = timeFormat + "-" + fileName;
                            var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                            uploadedFile.SaveAs(path);
                            string tempPath = "";
                            string finalPath = @"C:\uploads" + @"\" + projectId + @"\" + userDirectory + @"\" +
                                               moduleDirectory;

                            if (Directory.Exists(finalPath))
                            {
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                tempPath = finalPath + "\\" + fileName;
                                File.Delete(path);
                            }
                            else
                            {
                                Directory.CreateDirectory(finalPath);
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                tempPath = finalPath + "\\" + fileName;
                                File.Delete(path);
                            }

                            if (fileSavePath != "")
                            {
                                fileSavePath += "|" + tempPath;
                                //File.Delete(path);
                            }
                            else
                            {
                                fileSavePath = tempPath;
                                // File.Delete(path);
                            }
                        }

                    }

                }

                return fileSavePath;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "failed";
            }
        }

        public string GetFile(string path)
        {
            const string webServerFilePath = @"../Content/UploadImage";
            var fileName = Path.GetFileName(path);
            try
            {
                if (path != null)
                {
                    if (fileName == null) return null;
                    string webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath),
                        fileName);
                    if (webServerFileName != "" && !File.Exists(webServerFileName))
                    {
                        File.Copy(path, webServerFileName, true);
                        //File.Delete(path);
                    }
                }
                return webServerFilePath + "/" + fileName;
            }
            catch (Exception)
            {
                const string webServerFilePath2 = @"/Content/UploadImage";
                fileName = Path.GetFileName(path);
                try
                {
                    if (path != null)
                    {
                        if (fileName == null) return null;
                        var webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath2),
                            fileName);
                        if (webServerFileName != "" && !File.Exists(webServerFileName))
                        {
                            File.Copy(path, webServerFileName, true);
                            //File.Delete(path);
                        }
                    }
                    return webServerFilePath2 + "/" + fileName;
                }
                catch (Exception)
                {
                    const string webServerFilePath3 = @"~/Content/UploadImage";
                    fileName = Path.GetFileName(path);
                    try
                    {
                        if (path != null)
                        {
                            if (fileName == null) return null;
                            var webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath3),
                                fileName);
                            if (webServerFileName != "" && !File.Exists(webServerFileName))
                            {
                                File.Copy(path, webServerFileName, true);
                                //File.Delete(path);
                            }
                        }
                        return webServerFilePath3 + "/" + fileName;
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
            }
        }

        public string GetDiscussionFile(string path)
        {
            const string webServerFilePath = @"/Content/UploadImage";
            var fileName = Path.GetFileName(path);
            try
            {
                if (path != null)
                {
                    if (fileName == null) return null;
                    string webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath), fileName);
                    if (!File.Exists(webServerFileName))
                    {
                        File.Copy(path, webServerFileName, true);
                    }
                }
                return webServerFilePath + "/" + fileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetPoFeedbackFile(string path)
        {
            const string webServerFilePath = @"../../Content/UploadImage";
            var fileName = Path.GetFileName(path);
            try
            {
                if (path != null)
                {
                    if (fileName == null) return null;
                    string webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath), fileName);
                    if (!File.Exists(webServerFileName))
                    {
                        File.Copy(path, webServerFileName, true);
                    }
                }
                return webServerFilePath + "/" + fileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        //public string GetFile1(string path)
        //{
        //    if (!string.IsNullOrWhiteSpace(path))
        //    {
        //        const string webServerFilePath = @"../Content/UploadImage";
        //        var fileName = Path.GetFileName(path);
        //        string webServerFileName = Path.Combine(HttpContext.Current.Server.MapPath(webServerFilePath), fileName);
        //        if (!File.Exists(webServerFileName))
        //        {

        //            File.Copy(path, webServerFileName, true);

        //            return webServerFilePath + "/" + fileName;
        //            //return webServerFileName;
        //        }
        //        return webServerFilePath + "/" + fileName;
        //    }
        //    return string.Empty;

        //}

        //public string GetFile1(string path)
        //{
        //    string webServerFileName = "";

        //    const string webServerFilePath = @"/Content/UploadImage";
        //    var fileName = Path.GetFileName(path);
        //    if (path != null)
        //    {
        //        //fileList.Add(String.Format("{0}", System.IO.Path.GetFileName(item)));

        //        webServerFileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(webServerFilePath), fileName);
        //        if (!System.IO.File.Exists(webServerFileName))
        //        {

        //            System.IO.File.Copy(path, webServerFileName, true);

        //            return webServerFilePath + "/" + fileName;
        //            //return webServerFileName;
        //        }


        //    }


        //    return webServerFilePath + "/" + fileName;

        //}
        public string GetExtension(string path)
        {
            var filePath = path;
            string extension = Path.GetExtension(filePath);
            //File.Delete(path);
            return extension;
        }

        public string GetFileName(string path)
        {
            var filePath = path;
            string extension = Path.GetFileName(filePath);
            return extension;
        }
        public string UserProfilePictureUpload(long cmnUserId, string userDirectory, HttpPostedFileBase profilePicture)
        {
            if (cmnUserId > 0)
            {
                try
                {

                    HttpPostedFileBase uploadedFile = profilePicture;
                    var image = Image.FromStream(profilePicture.InputStream, true, true);
                    Image squareImage;
                    if (image.Width != image.Height)
                    {

                        if (image.Width > image.Height)
                        {
                            squareImage = image.Height > 500 ? ResizeImage(image, 500, 500) : ResizeImage(image, image.Height, image.Height);
                        }
                        else
                        {
                            squareImage = image.Height > 500 ? ResizeImage(image, 500, 500) : ResizeImage(image, image.Width, image.Width);
                        }
                    }
                    else
                    {
                        squareImage = image.Height > 500 ? ResizeImage(image, 500, 500) : ResizeImage(image, image.Width, image.Width);
                    }
                    const string initialPath = @"~/Content/UploadImage";
                    var fileSavePath = string.Empty;

                    if (uploadedFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(uploadedFile.FileName);
                        if (fileName != null)
                        {
                            var todayTime = DateTime.Now;
                            var time = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, todayTime.Hour,
                                todayTime.Minute, todayTime.Second);
                            var timeFormat = string.Format("{0:yyyy-MM-dd_hh-mm-ss}", time);
                            fileName = timeFormat + "-" + fileName;
                            var path = Path.Combine(HttpContext.Current.Server.MapPath(initialPath), fileName);
                            //uploadedFile.SaveAs(path);
                            squareImage.Save(path);
                            string tempPath = "";
                            string finalPath = @"C:\uploads" + @"\Users\" + userDirectory + @"\" + cmnUserId;
                            if (Directory.Exists(finalPath))
                            {
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                tempPath = finalPath + "\\" + fileName;
                                // File.Delete(path);
                            }
                            else
                            {
                                Directory.CreateDirectory(finalPath);
                                File.Copy(path, finalPath + "\\" + fileName, true);
                                tempPath = finalPath + "\\" + fileName;
                                // File.Delete(path);
                            }

                            if (fileSavePath != "")
                            {
                                fileSavePath += "|" + tempPath;
                                //File.Delete(path);
                            }
                            else
                            {
                                fileSavePath = tempPath;
                                // File.Delete(path);
                            }
                        }
                    }
                    return fileSavePath;
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    return "failed";
                }
            }
            return "ok";
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}