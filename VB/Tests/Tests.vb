Imports DevExpress.DataAccess
Imports DevExpress.DataAccess.DataFederation
Imports DevExpress.DataAccess.Native.DataFederation
Imports NUnit.Framework
Imports System.ComponentModel
Imports System.Linq

Namespace BindReportToFederatedDataSource.Tests
	<TestFixture>
	Public Class Tests
		<Test>
		Public Sub FederatedDataSourceConsistencyTest()
			Dim report = Form1.CreateReport()
			Dim federationDataSource = TryCast(report.DataSource, FederationDataSource)
			Assert.That(federationDataSource, [Is].Not.Null)

			Dim dataSources = federationDataSource.Sources.Select(Function(x) x.DataSource).Cast(Of DataComponentBase)().ToArray()
			Assert.That(dataSources.All(Function(ds) report.ComponentStorage.Contains(ds)), [Is].True)
		End Sub

		<Test>
		Public Sub FederationDataSource_ResultSetTest()
			Dim report = Form1.CreateReport()
			Dim federationDataSource = TryCast(report.DataSource, FederationDataSource)
			Assert.That(federationDataSource, [Is].Not.Null)
			federationDataSource.Fill()

			Dim result = DirectCast(DirectCast(federationDataSource, IListSource).GetList(), FederationResultSet)
			Assert.That(result.Tables.Count(), [Is].EqualTo(1))
			Dim table = result.Tables.First()
			Assert.That(table.Columns.Count, [Is].EqualTo(4))
			Assert.That(table.Count, [Is].EqualTo(77))
		End Sub
	End Class
End Namespace
