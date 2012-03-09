using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Provides services for managing learning module extensions.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-07-03</remarks>
    public class Extensions
    {
        #region Properties and Events
        /// <summary>
        /// The path to extract skins to.
        /// </summary>
        public static string SkinPath = null;

        public delegate void ExtensionEventHandler(object sender, ExtensionEventArgs e);

        /// <summary>
        /// Occurs when [execute extension].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public static event ExtensionEventHandler ExecuteExtension;

        /// <summary>
        /// Occurs when [inform user].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-09</remarks>
        public static event ExtensionEventHandler InformUser;

        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private IDictionary dictionary { get; set; }
        #endregion

        /// <summary>
        /// Extensionses the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        internal Extensions(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        #region Executed actions
        /// <summary>
        /// Saves the executed actions for each extension.
        /// </summary>
        private static Dictionary<Guid, List<ExtensionAction>> executedActions = new Dictionary<Guid, List<ExtensionAction>>();

        /// <summary>
        /// Remembers the last seen extension version.
        /// </summary>
        private static Dictionary<Guid, Version> extensionVersions = new Dictionary<Guid, Version>();

        /// <summary>
        /// Whether the lists have been modified since the last save/load.
        /// </summary>
        private static bool extensionListsDirty = false;

        /// <summary>
        /// Checks the extension version.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        private static void CheckExtensionVersion(IExtension extension)
        {
            if (!extensionVersions.ContainsKey(extension.Id))
            {
                extensionVersions[extension.Id] = extension.Version;
                extensionListsDirty = true;
            }
            else
            {
                if (extension.Version > extensionVersions[extension.Id])
                {
                    if (executedActions.ContainsKey(extension.Id))
                        executedActions.Remove(extension.Id);
                    extensionVersions[extension.Id] = extension.Version;
                    extensionListsDirty = true;
                }
            }
        }

        /// <summary>
        /// Determines whether action has already been executed.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private static bool ActionAlreadyExecuted(IExtension extension, ExtensionAction action)
        {
            if (!executedActions.ContainsKey(extension.Id) || executedActions[extension.Id] == null)
                return false;

            return executedActions[extension.Id].Contains(action);
        }

        /// <summary>
        /// Adds the action executed.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private static void AddActionExecuted(IExtension extension, ExtensionAction action)
        {
            if (!executedActions.ContainsKey(extension.Id) || executedActions[extension.Id] == null)
                executedActions[extension.Id] = new List<ExtensionAction>();

            if (!executedActions[extension.Id].Contains(action))
            {
                executedActions[extension.Id].Add(action);
                extensionListsDirty = true;
            }
        }
        #endregion

        #region Serialize & File functions
        /// <summary>
        /// Serializes to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Serialize(Stream stream)
        {
            if (!extensionListsDirty)
                return;

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, executedActions);
            formatter.Serialize(stream, extensionVersions);
            extensionListsDirty = false;
        }

        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Deserialize(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            executedActions = (Dictionary<Guid, List<ExtensionAction>>)formatter.Deserialize(stream);
            extensionVersions = (Dictionary<Guid, Version>)formatter.Deserialize(stream);
            extensionListsDirty = false;
        }

        /// <summary>
        /// Dumps to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Dump(string filename)
        {
            if (!extensionListsDirty)
                return;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));

                using (Stream stream = new FileStream(filename, FileMode.Create))
                    Serialize(stream);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Extension.Dump() - " + ex.Message);
            }
        }

        /// <summary>
        /// Restores from a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Restore(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    return;

                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    Deserialize(stream);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Extension.Restore() - " + ex.Message);
                try { File.Delete(filename); }
                catch { }
            }
        }
        #endregion

        #region Extension processing
        /// <summary>
        /// Processes this instance.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public void Process()
        {
            foreach (IExtension extension in dictionary.Extensions)
                ProcessExtension(extension);
        }

        /// <summary>
        /// Processes the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private void ProcessExtension(IExtension extension)
        {
            CheckExtensionVersion(extension);

            List<ExtensionAction> actions = new List<ExtensionAction>(extension.Actions);

            //sort install actions to be executed first, inform user actions to be executed last
            actions.Sort(delegate(ExtensionAction ea1, ExtensionAction ea2)
            {
                if (ea1.Kind == ExtensionActionKind.Install ^ ea2.Kind == ExtensionActionKind.Install)
                    return ea1.Kind == ExtensionActionKind.Install ? -1 : 1;

                if (ea1.Kind == ExtensionActionKind.InformUser ^ ea2.Kind == ExtensionActionKind.InformUser)
                    return ea1.Kind == ExtensionActionKind.InformUser ? 1 : -1;

                return 0;
            });

            try
            {
                foreach (ExtensionAction action in actions)
                {
                    if (action.Execution == ExtensionActionExecution.Never ||
                            (action.Execution == ExtensionActionExecution.Once && ActionAlreadyExecuted(extension, action)))
                        continue;

                    if (action.Kind == ExtensionActionKind.Install)
                        InstallExtension(extension, action);
                    else if (action.Kind == ExtensionActionKind.Force)
                        ForceExtension(extension, action);
                    else if (action.Kind == ExtensionActionKind.InformUser)
                        ExtensionInformUser(extension, action);

                    AddActionExecuted(extension, action);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Extension installation failed: " + e.ToString());
            }
        }
        #endregion

        #region Extension action handlers
        /// <summary>
        /// Installs the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private void InstallExtension(IExtension extension, ExtensionAction action)
        {
            string extractionPath;
            switch (extension.Type)
            {
                case ExtensionType.Skin:
                    extractionPath = SkinPath;
                    break;
                case ExtensionType.Unknown:
                default:
                    return;
            }
            extension.ExtractData(extractionPath);
        }

        /// <summary>
        /// Forces the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        private void ForceExtension(IExtension extension, ExtensionAction action)
        {
            if (ExecuteExtension != null)
                ExecuteExtension(this, new ExtensionEventArgs(extension, action));
        }

        /// <summary>
        /// Informs the user.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev02, 2009-07-09</remarks>
        private void ExtensionInformUser(IExtension extension, ExtensionAction action)
        {
            if (InformUser != null)
                InformUser(this, new ExtensionEventArgs(extension, action));
        }
        #endregion

    }

    /// <summary>
    /// EventArgs for the ExecuteExtension event.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-07-03</remarks>
    public class ExtensionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public IExtension Extension
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public ExtensionAction Action
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionEventArgs"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public ExtensionEventArgs(IExtension extension, ExtensionAction action)
        {
            this.Extension = extension;
            this.Action = action;
        }
    }
}
