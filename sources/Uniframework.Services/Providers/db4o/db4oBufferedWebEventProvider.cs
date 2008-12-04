using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.IO;
using System.Text;
using System.Web.Management;

namespace Uniframework.Services.db4oProviders
{
    public class db4oBufferedWebEventProvider : BufferedWebEventProvider
    {
        public static readonly string PROVIDER_NAME = "db4oBufferedWebEventProvider";
        private string buffer, bufferMode;

        private string connectionString;
        public IConnectionStringStore ConnectionStringStore = new ConfigurationManagerConnectionStringStore();

        private StringBuilder customInfo = new StringBuilder();
        private string providerName;

        public override void Initialize(string name, NameValueCollection config)
        {
            throw new NotImplementedException();

            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = PROVIDER_NAME;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "db4o ASP.NET Buffered Event provider");
            }

            base.Initialize(name, config);

            connectionString = ConnectionStringStore.GetConnectionString(config["connectionStringName"]);
            if (connectionString == null)
                throw new ProviderException("Connection string cannot be blank.");

            customInfo = new StringBuilder();

            providerName = name;
            buffer = config.Get("buffer");
            bufferMode = config.Get("bufferMode");

            customInfo.AppendLine(string.Format("Provider name: {0}", providerName));
            customInfo.AppendLine(string.Format("Buffering: {0}", buffer));
            customInfo.AppendLine(string.Format("Buffering modality: {0}", bufferMode));
        }

        // Processes the incoming events.
        // This method performs custom processing and, if
        // buffering is enabled, it calls the base.ProcessEvent
        // to buffer the event information.
        public override void ProcessEvent(WebBaseEvent eventRaised)
        {
            if (UseBuffering)
                base.ProcessEvent(eventRaised);
            else
            {
                // Buffering disabled, store event info
                // immediately.
                customInfo.AppendLine(string.Format("{0}", eventRaised.Message));

                // Store the information in the specified file.
                StoreToFile(customInfo, connectionString, FileMode.Append);
            }
        }

        // Processes the messages that have been buffered.
        // It is called by the ASP.NET when the flushing of 
        // the buffer is required according to the parameters 
        // defined in the <bufferModes> element of the 
        // <healthMonitoring> configuration section.
        public override void ProcessEventFlush(
            WebEventBufferFlushInfo flushInfo)
        {
            // Customize event information to be logged.
            customInfo.AppendLine(
                "SampleEventLogWebEventProvider buffer flush.");

            customInfo.AppendLine(
                string.Format("NotificationType: {0}",
                              flushInfo.NotificationType));

            customInfo.AppendLine(
                string.Format("EventsInBuffer: {0}",
                              flushInfo.EventsInBuffer));

            customInfo.AppendLine(
                string.Format("EventsDiscardedSinceLastNotification: {0}",
                              flushInfo.EventsDiscardedSinceLastNotification));


            // Read each buffered event and send it to the
            // Log.
            foreach (WebBaseEvent eventRaised in flushInfo.Events)
                customInfo.AppendLine(eventRaised.ToString());

            // Store the information in the specified file.
            StoreToFile(customInfo, connectionString, FileMode.Append);
        }

        // Performs standard shutdown.
        public override void Shutdown()
        {
            // Here you need the code that performs
            // those tasks required before shutting 
            // down the provider.

            // Flush the buffer, if needed.
            Flush();
        }

        // Store event information in a local file.
        private void StoreToFile(StringBuilder text,
                                 string filePath, FileMode mode)
        {
            int writeBlock;
            int startIndex;

            try
            {
                writeBlock = 256;
                startIndex = 0;

                // Open or create the local file 
                // to store the event information.
                FileStream fs = new FileStream(filePath,
                                               mode, FileAccess.Write);

                // Lock the file for writing.
                fs.Lock(startIndex, writeBlock);

                // Create a stream writer
                StreamWriter writer = new StreamWriter(fs);

                // Set the file pointer to the current 
                // position to keep adding data to it. 
                // If you want to rewrite the file use 
                // the following statement instead.
                // writer.BaseStream.Seek (0, SeekOrigin.Begin);
                writer.BaseStream.Seek(0, SeekOrigin.Current);

                //If the file already exists it must not 
                // be write protected otherwise  
                // the following write operation fails silently.
                writer.Write(text.ToString());

                // Update the underlying file
                writer.Flush();

                // Unlock the file for other processes.
                fs.Unlock(startIndex, writeBlock);

                // Close the stream writer and the underlying file     
                writer.Close();

                fs.Close();
            }
            catch (Exception e)
            {
                throw new Exception(
                    "SampleEventProvider.StoreToFile: "
                    + e);
            }
        }
    }
}