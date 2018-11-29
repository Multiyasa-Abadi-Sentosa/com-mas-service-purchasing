﻿using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.PDFTemplates
{
	public class VatPDFTemplate
	{
		public MemoryStream GeneratePdfTemplate(GarmentInvoiceViewModel viewModel, int clientTimeZoneOffset, IGarmentDeliveryOrderFacade DOfacade)
		{
			Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
			Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
			Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
			//Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

			Document document = new Document(PageSize.A4, 40, 40, 40, 40);
			document.AddHeader("Header", viewModel.npn);
			MemoryStream stream = new MemoryStream();
			PdfWriter writer = PdfWriter.GetInstance(document, stream);
			writer.PageEvent = new PDFPages();
			document.Open();

			Chunk chkHeader = new Chunk(" ");
			Phrase pheader = new Phrase(chkHeader);
			HeaderFooter header = new HeaderFooter(pheader, false);
			header.Border = Rectangle.NO_BORDER;
			header.Alignment = Element.ALIGN_RIGHT;
			document.Header = header;


			#region Header

			PdfPTable tableHeader = new PdfPTable(1);
			tableHeader.SetWidths(new float[] { 4f });
			PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
			//PdfPCell cellHeaderContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

			cellHeaderContentLeft.Phrase = new Phrase("PT DAN LIRIS" + "\n" + "Head Office: Kelurahan Banaran" + "\n" + "Kecamatan Grogol" + "\n" + "Sukoharjo 57193 - INDONESIA" + "\n" + "PO.BOX 166 Solo 57100" + "\n" + "Telp. (0271) 740888, 714400" + "\n" + "Fax. (0271) 735222, 740777", bold_font);
			tableHeader.AddCell(cellHeaderContentLeft);

			//string noPO = viewModel.Supplier.Import ? "FM-PB-00-06-009/R1" + "\n" + "PO: " + EPONo  : "PO: " + EPONo;


			PdfPCell cellHeader = new PdfPCell(tableHeader); // dont remove
			tableHeader.ExtendLastRow = false;
			tableHeader.SpacingAfter = 10f;
			document.Add(tableHeader);

			string titleString = "NOTA PAJAK PPN\n\n";
			Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
			document.Add(title);
			bold_font.SetStyle(Font.NORMAL);


			PdfPTable tableIncomeTax = new PdfPTable(1);
			tableIncomeTax.SetWidths(new float[] { 4.5f });
			PdfPCell cellTaxLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
			cellTaxLeft.Phrase = new Phrase("No. Nota Pajak" + "    : " + viewModel.npn, normal_font);
			tableIncomeTax.AddCell(cellTaxLeft);
			cellTaxLeft.Phrase = new Phrase("Kode Supplier" + "      : " + viewModel.supplier.Code, normal_font);
			tableIncomeTax.AddCell(cellTaxLeft);
			cellTaxLeft.Phrase = new Phrase("Nama Supplier" + "     :" + viewModel.supplier.Name, normal_font);
			tableIncomeTax.AddCell(cellTaxLeft);



			PdfPCell cellSupplier = new PdfPCell(tableIncomeTax); // dont remove
			tableIncomeTax.ExtendLastRow = false;
			tableIncomeTax.SpacingAfter = 10f;
			document.Add(tableIncomeTax);
			#endregion
			#region data
			PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
			PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
			PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

			PdfPTable tableContent = new PdfPTable(6);
			tableContent.SetWidths(new float[] { 4.5f, 5f, 3.5f, 4f, 2.2f, 5f });
			cellCenter.Phrase = new Phrase("No Surat Jalan", bold_font);
			tableContent.AddCell(cellCenter);
			cellCenter.Phrase = new Phrase("Tanggal Surat Jalan", bold_font);
			tableContent.AddCell(cellCenter);
			cellCenter.Phrase = new Phrase("No Invoice", bold_font);
			tableContent.AddCell(cellCenter);
			cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
			tableContent.AddCell(cellCenter);
			cellCenter.Phrase = new Phrase("Rate PPn", bold_font);
			tableContent.AddCell(cellCenter);
			cellCenter.Phrase = new Phrase("Sub Total PPn", bold_font);
			tableContent.AddCell(cellCenter);

			double total = 0;
			double totalPPN = 0;
			double totalPPNIDR = 0;
			foreach (GarmentInvoiceItemViewModel item in viewModel.items)
			{
				total += item.deliveryOrder.totalAmount;
				
				foreach (GarmentInvoiceDetailViewModel detail in item.details)
				{

					cellLeft.Phrase = new Phrase(item.deliveryOrder.doNo, normal_font);
					tableContent.AddCell(cellLeft);

					string doDate = item.deliveryOrder.doDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));

					cellLeft.Phrase = new Phrase(doDate, normal_font);
					tableContent.AddCell(cellLeft);

					cellLeft.Phrase = new Phrase(viewModel.invoiceNo, normal_font);
					tableContent.AddCell(cellLeft);

					cellLeft.Phrase = new Phrase(detail.product.Name, normal_font);
					tableContent.AddCell(cellLeft);

					cellRight.Phrase = new Phrase((10).ToString(), normal_font);
					tableContent.AddCell(cellRight);

					cellRight.Phrase = new Phrase( Math.Round(10 *  detail.pricePerDealUnit * detail.doQuantity / 100,2).ToString("N2"), normal_font);
					tableContent.AddCell(cellRight);
					totalPPN += ((0.1) * detail.pricePerDealUnit * detail.doQuantity);
					var garmentDeliveryOrder = DOfacade.ReadById((int)item.deliveryOrder.Id);
					double rate = 1;
					if(garmentDeliveryOrder !=null)
					{ rate = (double)garmentDeliveryOrder.DOCurrencyRate; }
					totalPPNIDR += (0.1 * detail.pricePerDealUnit * detail.doQuantity  * rate);/**dikali rate DO*/
				}

			}
			 
			cellRight.Phrase = new Phrase("Total Ppn", normal_font);
			cellRight.Colspan = 5;
			tableContent.AddCell(cellRight);
			cellRight.Phrase = new Phrase(totalPPN.ToString("N2"), normal_font);
			cellRight.Colspan = 5;
			tableContent.AddCell(cellRight);
			cellRight.Phrase = new Phrase("Total Ppn IDR", normal_font);
			cellRight.Colspan = 5;
			tableContent.AddCell(cellRight);
			cellRight.Phrase = new Phrase( totalPPNIDR.ToString("N2"), normal_font);
			cellRight.Colspan = 5;
			tableContent.AddCell(cellRight);

			PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
			tableContent.ExtendLastRow = false;
			tableContent.SpacingAfter = 20f;
			document.Add(tableContent);
			#endregion
			#region TableSignature

			PdfPTable tableSignature = new PdfPTable(2);

			PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

			cellSignatureContent.Phrase = new Phrase("Administrasi\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
			tableSignature.AddCell(cellSignatureContent);
			cellSignatureContent.Phrase = new Phrase("Staff Pembelian\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
			tableSignature.AddCell(cellSignatureContent);
			cellSignatureContent.Phrase = new Phrase("Verifikasi\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
			tableSignature.AddCell(cellSignatureContent);


			PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
			tableSignature.ExtendLastRow = false;
			tableSignature.SpacingBefore = 20f;
			tableSignature.SpacingAfter = 20f;
			document.Add(tableSignature);

			#endregion
			document.Close();
			byte[] byteInfo = stream.ToArray();
			stream.Write(byteInfo, 0, byteInfo.Length);
			stream.Position = 0;

			return stream;
		}
	}
}

