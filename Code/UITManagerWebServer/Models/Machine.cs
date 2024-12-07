﻿namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a machine, which can be associated with alarms and notes.
    /// </summary>
    public class Machine {
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<Alarm> Alarms { get; set; }

        public List<Note> Notes { get; set; }

        public List<Information> Informations { get; set; }

        public Boolean IsWorking { get; set; }

        public string? Model { get; set; }

        public DateTime LastSeen { get; set; }

        public Machine() {
            Alarms = new List<Alarm>();
            Notes = new List<Note>();
            Informations = new List<Information>();
        }

        /// <summary>
        /// Gets the most recent note entry of the machine.
        /// The latest note is determined by the most recent creation date.
        /// </summary>
        /// <returns>The most recent note of the machine, or <c>null</c> if no note is available.</returns>
        public Note GetLatestNote() {
            return Notes.OrderByDescending(n => n.CreatedAt).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the operating system name from the machine's information.
        /// </summary>
        /// <returns>The OS name if available; otherwise, <c>null</c>.</returns>
        public string GetOsName() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osName = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "OS Name");

            return osName?.Value;
        }

        /// <summary>
        /// Retrieves the operating system version from the machine's information.
        /// </summary>
        /// <returns>The OS version if available; otherwise, <c>null</c>.</returns>
        public string GetOsVersion() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osVersion = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Os Version");

            return osVersion?.Value;
        }

        /// <summary>
        /// Retrieves the operating system build number from the machine's information.
        /// </summary>
        /// <returns>The OS build number if available; otherwise, <c>null</c>.</returns>
        public string GetOsBuild() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osBuild = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Os Build");

            return osBuild?.Value;
        }

        /// <summary>
        /// Retrieves the service tag from the machine's information.
        /// </summary>
        /// <returns>The service tag if available; otherwise, <c>null</c>.</returns>
        public string GetServiceTag() {
            Value? serviceTag = Informations
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Tag Service");

            return serviceTag?.Value;
        }
        public string GetInformationValueByName(string name)
        {
            return FindInformationValue(Informations, name);
        }

        private string FindInformationValue(List<Information> informations, string name)
        {
            foreach (var info in informations)
            {
                if (info is Value value && value.Name == name)
                {
                    return value.Values;
                }

                if (info is Component component)
                {
                    var result = FindInformationValue(component.Children, name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}