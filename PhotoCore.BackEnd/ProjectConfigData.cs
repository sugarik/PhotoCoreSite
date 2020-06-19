using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class ProjectConfigData
    {
        public string awsAccessKeyId { get { return "AKIA4IDEBL4UTXBFRFPG"; } }
        public string awsSecretAccessKey { get { return "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"; } }
        public string awsEndpoint { get { return "eu-west-1"; } }
        public string awsCollectionId { get { return "lebyazhiy"; } }
        public float awsFaceMatchThreshold { get { return 90; } }
        public float awsSimilarityLevel { get { return 70; } }

        public string dbDataSource { get { return "mssql.by1880.hb.by"; } }
        public string dbInitialCatalog { get { return "photocore"; } }
        public bool dbPersistSecurityInfo { get { return true; } }
        public string dbUserId { get { return "photocoreuser"; } }
        public string dbPassword { get { return "!Q@WAZSX3e4rdcfv"; } }
        public bool dbPooling { get { return true; } }

        public float[] billCosts { get { return new float[] { 0, 5, 10, 15, 19, 23, 27, 30, 33, 36, 38, 40, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 }; } }
        public string billCurrency { get { return "BYN"; } }
        public string billOrderPrefix { get { return "LEBYAZHIY_"; } }
        public string billUserName { get { return "Testing"; } }
        public string billPassword { get { return "Testing123"; } }

        public string billReturnUrl { get { return "finish.html"; } }
        public string billFailUrl { get { return "error.html"; } }

        public string billSslCertPath { get { return "mpi-test.bgpb.by.crt"; } }
        public string billSslKeyPath { get { return "mpi.test.key"; } }
        public string billSslKeyPass { get { return "Bgpb2019"; } }



    }
}
