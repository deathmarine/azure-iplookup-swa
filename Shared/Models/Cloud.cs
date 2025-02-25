using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BlazorApp.Shared.Models {
    public class Cloud {

        public int changeNumber {
            get; set;
        }
        public string cloud {
            get; set;
        }
        public ServiceTag[] values {
            get; set;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ServiceTag {
        public string name {
            get; set;
        }
        public string id {
            get; set;
        }
        public Properties properties {
            get; set;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }

    public class Properties {
        public int changeNumber {
            get; set;
        }
        public string region {
            get; set;
        }
        public int regionId {
            get; set;
        }
        public string platform {
            get; set;
        }
        public string systemService {
            get; set;
        }
        public string[] addressPrefixes {
            get; set;
        }
        public string[] networkFeatures {
            get; set;
        }
    }

}
