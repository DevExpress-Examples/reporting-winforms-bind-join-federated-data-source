using DevExpress.DataAccess;
using DevExpress.DataAccess.DataFederation;
using DevExpress.DataAccess.Native.DataFederation;
using NUnit.Framework;
using System.ComponentModel;
using System.Linq;

namespace BindReportToFederatedDataSource.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        public void FederatedDataSourceConsistencyTest() {
            var report = Form1.CreateReport();
            var federationDataSource = report.DataSource as FederationDataSource;
            Assert.That(federationDataSource, Is.Not.Null);

            var dataSources = federationDataSource.Sources
                .Select(x => x.DataSource)
                .Cast<DataComponentBase>().ToArray();
            Assert.That(dataSources.All(ds => report.ComponentStorage.Contains(ds)), Is.True);
        }

        [Test]
        public void FederationDataSource_ResultSetTest() {
            var report = Form1.CreateReport();
            var federationDataSource = report.DataSource as FederationDataSource;
            Assert.That(federationDataSource, Is.Not.Null);
            federationDataSource.Fill();

            var result = (FederationResultSet)((IListSource)federationDataSource).GetList();
            Assert.That(result.Tables.Count(), Is.EqualTo(1));
            var table = result.Tables.First();
            Assert.That(table.Columns.Count, Is.EqualTo(4));
            Assert.That(table.Count, Is.EqualTo(77));
        }
    }
}
