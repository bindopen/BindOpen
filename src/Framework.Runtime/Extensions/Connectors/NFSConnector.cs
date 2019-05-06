﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BindOpen.Framework.Core.Extensions.Attributes;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Runtime.Extensions.Carriers;
using System.IO;

namespace BindOpen.Framework.Runtime.Extensions.Connectors
{
    /// <summary>
    /// This class represents a file NFS connector.
    /// </summary>
    [Connector(Name = "runtime$nfsConnector")]
    public class NFSConnector : RepositoryConnector
    {
        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the NFSConnector class.
        /// </summary>
        public NFSConnector() : base()
        {
        }

        #endregion

        // -----------------------------------------------
        // MANAGEMENT
        // -----------------------------------------------

        #region Management

        // Open / Close ---------------------------------------

        /// <summary>
        /// Opens a connection.
        /// </summary>
        public override ILog Open()
        {
            return base.Open();
        }

        /// <summary>
        /// Closes the existing connection.
        /// </summary>
        public override ILog Close()
        {
            return base.Close();
        }

        // Pull ---------------------------------------

        /// <summary>
        /// Gets a remote file to a local Uri.
        /// </summary>
        /// <param name="remoteFileUri">The remote Uri to consider.</param>
        /// <param name="localPathUri">The Uri of the local path to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <param name="canOverwrite">Indicates whether the local file can be overwritten.</param>
        public override void Pull(
           String remoteFileUri,
           String localPathUri,
           Boolean canOverwrite,
            ILog log = null)
        {
            log = log ?? new Log();

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPathUri)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPathUri));

                File.Copy(remoteFileUri, localPathUri, canOverwrite);
            }
            catch (Exception exception)
            {
                ILogEvent logEvent = log.AddException(exception);
            }
        }

        // Push ---------------------------------------

        /// <summary>
        /// Posts a local file to a remote Uri.
        /// </summary>
        /// <param name="localFileUri">The local Uri to consider.</param>
        /// <param name="remotePathUri">The Uri of the remote path to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <param name="canOverwrite">Indicates whether the remote file can be overwritten.</param>
        public override void Push(
           String localFileUri,
           String remotePathUri,
           Boolean canOverwrite,
            ILog log = null)
        {
            log = log ?? new Log();

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(remotePathUri)))
                    Directory.CreateDirectory(Path.GetDirectoryName(remotePathUri));

                File.Copy(localFileUri, remotePathUri, canOverwrite);
            }
            catch (Exception exception)
            {
                log.AddException(exception);
            }
        }

        // Browser ---------------------------------------

        /// <summary>
        /// Waits for the specified file to be accessible.
        /// </summary>
        /// <param name="path">The path of the file to consider.</param>
        /// <param name="aSecondNumber">The number of seconds to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>Returns true if the file is available. False otherwise.</returns>
        public static Boolean WaitForFile(
            String path,
            int aSecondNumber = 4,
            ILog log = null)
        {
            log = log ?? new Log();

            if (string.IsNullOrEmpty(path))
                return false;

            DateTime dateTime = DateTime.Now.AddSeconds(aSecondNumber);
            Boolean isFileAccessible = !File.Exists(path);

            while ((DateTime.Now <= dateTime) && (!isFileAccessible))
            {
                FileStream fileStream = null;
                try
                {
                    FileInfo fileInfo = new FileInfo(path);
                    fileStream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    isFileAccessible = true;
                }
                catch
                {
                    isFileAccessible = false;
                }
                finally
                {
                    fileStream?.Close();
                }
            }

            return isFileAccessible;
        }

        /// <summary>
        /// Gets the list of elements of the remote folder.
        /// </summary>
        /// <param name="folderUri">The Uri of the folder path to consider.</param>
        /// <param name="filter">The filter to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <param name="isRecursive">Indicates whether the search is folder recursive.</param>
        /// <param name="fileKind">The kind of files to consider.</param>
        /// <returns>Lists of elements of the remote folder.</returns>
        public override List<RepositoryItem> GetFiles(
           String folderUri,
           String filter,
           Boolean isRecursive,
           ILog log = null,
           CarrierKind_standard fileKind = CarrierKind_standard.Any)
        {
            log = log ?? new Log();

            Boolean isRegularExpression = ((!string.IsNullOrEmpty(filter)) && (filter.StartsWith("/")));
            Regex regex = null;

            List<RepositoryItem> files = new List<RepositoryItem>();
            if (Directory.Exists(folderUri))
            {
                if ((fileKind == CarrierKind_standard.File) |
                    (fileKind == CarrierKind_standard.Any))
                {
                    FileInfo[] fileInfos = null;

                    if (!isRegularExpression)
                        fileInfos = (new DirectoryInfo(folderUri)).GetFiles((filter ?? "*.*"), (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                    else
                    {
                        filter = filter.Substring(1);
                        fileInfos = (new DirectoryInfo(folderUri)).GetFiles("*.*", (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                        try
                        {
                            regex = new Regex(filter);
                        }
                        catch
                        {
                            isRegularExpression = false;
                        }
                    }

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        Boolean isFound = !isRegularExpression;
                        if ((isRegularExpression) & (regex != null))
                            isFound = regex.IsMatch(fileInfo.Name);

                        if (isFound)
                        {
                            Carriers.RepositoryFile file = new Carriers.RepositoryFile()
                                {
                                    Name = fileInfo.Name,
                                    Path = fileInfo.FullName,
                                    CreationDate = fileInfo.CreationTime.ToString(),
                                    LastAccessDate = fileInfo.LastAccessTime.ToString(),
                                    LastWriteDate = fileInfo.LastWriteTime.ToString(),
                                    Length = (ulong)fileInfo.Length,
                                    ParentPath = folderUri
                                };
                            files.Add(file);
                        }
                    }
                }
                if ((fileKind == CarrierKind_standard.Folder) |
                    (fileKind == CarrierKind_standard.Any))
                {
                    DirectoryInfo[] directoryInfos = null;

                    if (!isRegularExpression)
                        directoryInfos = (new DirectoryInfo(folderUri)).GetDirectories((filter ?? "*.*"), (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                    else
                    {
                        filter = filter.Substring(1);
                        directoryInfos = (new DirectoryInfo(folderUri)).GetDirectories("*.*", (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                        try
                        {
                            regex = new Regex(filter);
                        }
                        catch
                        {
                            isRegularExpression = false;
                        }
                    }

                    foreach (DirectoryInfo directoryInfo in directoryInfos)
                    {
                        Boolean isFound = !isRegularExpression;
                        if (isRegularExpression & (regex != null))
                            isFound = regex.IsMatch(directoryInfo.Name);

                        if (isFound)
                        {
                            Carriers.RepositoryFolder folder = new Carriers.RepositoryFolder()
                            {
                                Name = directoryInfo.Name,
                                Path = directoryInfo.FullName,
                                CreationDate = directoryInfo.CreationTime.ToString(),
                                LastAccessDate = directoryInfo.LastAccessTime.ToString(),
                                LastWriteDate = directoryInfo.LastWriteTime.ToString(),
                                ParentPath = folderUri
                            };
                            files.Add(folder);
                        }
                    }
                }
            }

            return files;
        }

        // Delete ---------------------------------------------------

        /// <summary>
        /// Deletes a local file.
        /// </summary>
        /// <param name="localFileUri">The local Uri to consider.</param>
        /// <param name="log">The log to consider.</param>
        public static void DeleteFile(
            String localFileUri,
            ILog log = null)
        {
            log = log ?? new Log();

            try
            {
                if (File.Exists(localFileUri))
                {
                    Directory.Delete(localFileUri, true);
                    log.AddMessage("File '" + localFileUri + "' deleted");
                }
                else
                    log.AddError("Could not delete file '" + localFileUri + "'");
            }
            catch (Exception exception)
            {
                log.AddException(exception);
            }
        }

        /// <summary>
        /// Deletes a local folder.
        /// </summary>
        /// <param name="localfolderUri">The local Uri to consider.</param>
        /// <param name="log">The log to consider.</param>
        public static void DeleteFolder(
            String localfolderUri,
            ILog log = null)
        {
            log = log ?? new Log();

            try
            {
                if (Directory.Exists(localfolderUri))
                {
                    Directory.Delete(localfolderUri, true);
                    log.AddMessage("Folder '" + localfolderUri + "' deleted");
                }
                else
                    log.AddError("Could not delete folder '" + localfolderUri + "'");
            }
            catch (Exception exception)
            {
                log.AddException("Could not delete folder '" + localfolderUri + "'", description: exception.ToString());
            }
        }

        /// <summary>
        /// Deletes the items.
        /// </summary>
        /// <param name="folderUri">The Uri of the folder path to consider.</param>
        /// <param name="filter">The filter to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <param name="timeLimit">The date time limit to consider.</param>
        /// <param name="isRecursive">Indicates whether the search is folder recursive.</param>
        /// <param name="fileKind">The kind of elements to consider.</param>
        public override void DeleteItems(
            String folderUri,
            String filter,
            DateTime timeLimit,
            Boolean isRecursive,
            ILog log = null,
            CarrierKind_standard fileKind = CarrierKind_standard.Any)
        {
            log = log ?? new Log();

            foreach (RepositoryItem item in this.GetFiles(
                folderUri, filter, isRecursive, log, fileKind))
            {
                if (item.LastWriteDate != null)
                {
                    DateTime lastWriteDateTime;
                    if ((timeLimit == null) ||
                        ((DateTime.TryParse(item.LastWriteDate, out lastWriteDateTime)) &&
                        (DateTime.Now.Subtract(lastWriteDateTime).Ticks > timeLimit.Ticks)))
                        if (item is RepositoryFolder)
                            NFSConnector.DeleteFolder(item.Path, log);
                        else if (item is RepositoryFile)
                            NFSConnector.DeleteFile(item.Path, log);
                }
            }
        }

        #endregion
    }
}