using DnsClient.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared.Models {

    public class DNSLookup {
        public string HostName {
            get; set;
        }
        public string[] AddressList {
            get; set;
        }
        public Record[] Records {
            get; set;
        }
    }

    public class Record {
        public Domainname DomainName {
            get; set;
        }
        public int RecordType {
            get; set;
        }
        public int RecordClass {
            get; set;
        }
        public int TimeToLive {
            get; set;
        }
        public int InitialTimeToLive {
            get; set;
        }
        public int RawDataLength {
            get; set;
        }
    }

    public class Domainname {
        public string Original {
            get; set;
        }
        public string Value {
            get; set;
        }
        public object NumberOfLabels {
            get; set;
        }
    }
}
