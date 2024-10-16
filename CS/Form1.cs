using System;
using System.Windows.Forms;
#region usings_sql
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
#endregion
#region using_excel
using DevExpress.DataAccess.Excel;
#endregion
#region using_datafederation
using DevExpress.DataAccess.DataFederation;
#endregion
#region usings_report
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports.UI;
using System.IO;
#endregion

namespace BindReportToFederatedDataSource {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		void Button1_Click(object sender, EventArgs e) {
			#region ShowDesigner
			ReportDesignTool designTool = new ReportDesignTool(CreateReport());
			designTool.ShowRibbonDesignerDialog();
			#endregion
		}
		#region CreateFederationDataSource
		static FederationDataSource CreateFederationDataSource(SqlDataSource sql, ExcelDataSource excel) {
			// Create a federated query's SQL and Excel sources.
			Source sqlSource = new Source(sql.Name, sql, "Categories");
			Source excelSource = new Source(excel.Name, excel, "");

			// Create a federated query.
			var selectNode = sqlSource.From()
				// Select the "CategoryName" column from the SQL source.
				.Select("CategoryName")
				// Join the SQL source with the Excel source based on the "CategoryID" key field.
				.Join(excelSource, "[Excel_Products.CategoryID] = [Sql_Categories.CategoryID]")
				// Select columns from the Excel source.
				.Select("CategoryID", "ProductName", "UnitPrice")
				// Specify the query's name and build it. 
				.Build("CategoriesProducts");

			// Create a federated data source and add the federated query to the collection.
			var federationDataSource = new FederationDataSource();
			federationDataSource.Queries.Add(selectNode);
			// Build the data source schema to display it in the Field List.
			federationDataSource.RebuildResultSchema();

			return federationDataSource;
		}
		#endregion
		#region CreateReport
		public static XtraReport CreateReport() {
			// Create a new report.
			var report = new XtraReport();

			// Create data sources. 
			var sqlDataSource = CreateSqlDataSource();
			var excelDataSource = CreateExcelDataSource();
			var federationDataSource = CreateFederationDataSource(sqlDataSource, excelDataSource);
			// Add all data sources to the report to avoid serialization issues. 
			report.ComponentStorage.AddRange(new IComponent[] { sqlDataSource, excelDataSource, federationDataSource });
			// Assign a federated data source to the report.
			report.DataSource = federationDataSource;
			report.DataMember = "CategoriesProducts";

			// Add the Detail band and two labels bound to the federated data source's fields.
			var detailBand = new DetailBand() { HeightF = 50 };
			report.Bands.Add(detailBand);
			var categoryLabel = new XRLabel() { WidthF = 150 };
			var productLabel = new XRLabel() { WidthF = 300, LocationF = new PointF(200, 0) };
			categoryLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[CategoryName]"));
			productLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
			detailBand.Controls.AddRange(new[] { categoryLabel, productLabel });

			return report;
		}
		#endregion
		#region CreateSqlDataSource
		static SqlDataSource CreateSqlDataSource() {
			var connectionParameters = new SQLiteConnectionParameters("Data/nwind.db", null); 
			var sqlDataSource = new SqlDataSource(connectionParameters) { Name = "Sql_Categories" };
			var categoriesQuery = SelectQueryFluentBuilder.AddTable("Categories").SelectAllColumnsFromTable().Build("Categories");
			sqlDataSource.Queries.Add(categoriesQuery);
			sqlDataSource.RebuildResultSchema();
			return sqlDataSource;
		}
		#endregion
		#region CreateExcelDataSource
		static ExcelDataSource CreateExcelDataSource() {
			var excelDataSource = new ExcelDataSource() { Name = "Excel_Products" };
			excelDataSource.FileName = Path.Combine(Path.GetDirectoryName(typeof(Form1).Assembly.Location), "Data/Products.xlsx");
			excelDataSource.SourceOptions = new ExcelSourceOptions() {
				ImportSettings = new ExcelWorksheetSettings("Sheet"),
			};
			excelDataSource.RebuildResultSchema();
			return excelDataSource;
		}
		#endregion
	}
}