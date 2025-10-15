using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class ShippingLabelGenerator : ILabelGenerator
{
    static ShippingLabelGenerator()
    {
        // Configure QuestPDF license (Community license is free)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateLabel(
        string trackingNumber,
        CarrierType carrier,
        Address fromAddress,
        Address toAddress,
        decimal weight,
        string orderReference)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, trackingNumber, carrier, fromAddress, toAddress, weight, orderReference));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("ÉTIQUETTE D'EXPÉDITION").FontSize(20).Bold();
                column.Item().Text("SHIPPING LABEL - SIMULÉE").FontSize(10).Italic();
            });

            row.ConstantItem(100).AlignRight().Text(text =>
            {
                text.Span($"{DateTime.Now:dd/MM/yyyy}").FontSize(10);
            });
        });
    }

    void ComposeContent(
        IContainer container,
        string trackingNumber,
        CarrierType carrier,
        Address fromAddress,
        Address toAddress,
        decimal weight,
        string orderReference)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Carrier and tracking number
            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(col =>
            {
                col.Item().Text($"Transporteur: {GetCarrierName(carrier)}").FontSize(14).Bold();
                col.Item().Text($"Numéro de suivi: {trackingNumber}").FontSize(16).Bold();
                col.Item().Text($"Référence commande: {orderReference}").FontSize(10);
            });

            // Barcode simulation
            column.Item().PaddingVertical(10).Border(1).BorderColor(Colors.Black)
                .Height(60).AlignCenter().AlignMiddle()
                .Text($"|||  {trackingNumber}  |||").FontSize(12).FontFamily("Courier New");

            // From address
            column.Item().Border(1).Padding(10).Column(col =>
            {
                col.Item().Text("EXPÉDITEUR / FROM").FontSize(10).Bold();
                col.Item().Text(fromAddress.Street).FontSize(10);
                col.Item().Text($"{fromAddress.ZipCode} {fromAddress.City}").FontSize(10);
                col.Item().Text(fromAddress.Country).FontSize(10).Bold();
            });

            // To address (bigger)
            column.Item().Border(2).BorderColor(Colors.Black).Padding(15).Column(col =>
            {
                col.Item().Text("DESTINATAIRE / TO").FontSize(12).Bold();
                col.Item().PaddingTop(5).Text(toAddress.Street).FontSize(14);
                col.Item().Text($"{toAddress.ZipCode} {toAddress.City}").FontSize(14).Bold();
                col.Item().Text(toAddress.State ?? "").FontSize(12);
                col.Item().Text(toAddress.Country).FontSize(12).Bold();
            });

            // Package info
            column.Item().Row(row =>
            {
                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().Text("POIDS / WEIGHT").FontSize(8).Bold();
                    col.Item().Text($"{weight:F2} kg").FontSize(12);
                });

                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().Text("SERVICE").FontSize(8).Bold();
                    col.Item().Text(GetServiceType(carrier)).FontSize(12);
                });

                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().Text("DATE D'EXPÉDITION").FontSize(8).Bold();
                    col.Item().Text($"{DateTime.Now:dd/MM/yyyy}").FontSize(12);
                });
            });

            // Warning
            column.Item().PaddingTop(20).Background(Colors.Yellow.Lighten3).Padding(10)
                .Text("⚠️ ÉTIQUETTE SIMULÉE - Pour test uniquement").FontSize(10).Bold().Italic();
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Généré par le système de gestion des colis • ").FontSize(8);
            text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
        });
    }

    string GetCarrierName(CarrierType carrier)
    {
        return carrier switch
        {
            CarrierType.LaPoste => "La Poste",
            CarrierType.Colissimo => "Colissimo",
            CarrierType.Chronopost => "Chronopost",
            CarrierType.MondialRelay => "Mondial Relay",
            CarrierType.DHL => "DHL Express",
            CarrierType.UPS => "UPS",
            CarrierType.FedEx => "FedEx",
            _ => "Standard"
        };
    }

    string GetServiceType(CarrierType carrier)
    {
        return carrier switch
        {
            CarrierType.Chronopost => "Express 24h",
            CarrierType.MondialRelay => "Point Relais",
            CarrierType.DHL => "Express International",
            CarrierType.UPS => "Standard",
            CarrierType.FedEx => "Priority",
            _ => "Standard"
        };
    }
}
