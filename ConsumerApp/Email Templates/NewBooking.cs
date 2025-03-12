using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp.Email_Templates;
class NewBooking
{
    public string NewBookingXlst()
    {
        return @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
                        <xsl:template match=""/"">
                            <html>
                                <head>
                                    <title>Reserva XML</title>
                                    <style>
                                        body {
                                            margin: 0;
                                            padding: 0;
                                            -webkit-text-size-adjust: 100%;
                                            -ms-text-size-adjust: 100%;
                                            font-family: Arial, sans-serif;
                                            background-color: #f4f4f4;
                                            text-align: center;
                                            }
                                        .content {
                                            width: 50%;
                                            -webkit-text-size-adjust: 100%;
                                            -ms-text-size-adjust: 100%;
                                            background-color: #f9f9f9;
                                            padding: 20px;
                                            border: 1px solid #ddd;
                                            border-radius: 10px;
                                            text-align: left
                                        }
                                        .title {
                                            font-size: 20px;
                                            font-weight: bold;
                                            margin-bottom: 10px;
                                            color: #007BFF;
                                        }
                                        .item {
                                            font-size: 16px;
                                            margin: 8px 0;
                                            padding: 10px;
                                            border-bottom: 1px solid #ddd;
                                        }
                                        .footer {
                                            margin-top: 20px;
                                            font-style: italic;
                                            color: #555;
                                        }   
                                    </style>
                                </head>
                                <body>                
                                    <div class=""content"">
                                        <div class=""title"">Detalhes da Reserva - <xsl:value-of select=""BookingShared/BookingId""/></div>
                
                                        <div class=""item""><b>Voucher: </b> <xsl:value-of select=""BookingShared/BookingId""/></div>
                                        <div class=""item""><b>Nome do Viajante: </b> <xsl:value-of select=""BookingShared/TravelerFullName""/></div>
                                        <div class=""item""><b>Nome do Quarto: </b> <xsl:value-of select=""BookingShared/RoomName""/></div>
                                        <div class=""item""><b>Tipo do Quarto: </b> <xsl:value-of select=""BookingShared/TypeRoom""/></div>
                                        <div class=""item""><b>Check-in: </b> <xsl:value-of select=""BookingShared/CheckIn""/></div>
                                        <div class=""item""><b>Check-out: </b> <xsl:value-of select=""BookingShared/CheckOut""/></div>
                                        <div class=""item""><b>Total: </b> <xsl:value-of select=""BookingShared/TotalPrice""/></div>
                                        <div class=""item""><b>Status: </b> <xsl:value-of select=""BookingShared/Status""/></div>
                                    </div>
                                    <p class=""footer"">Se possível, por favor, imprima este e-mail para referência futura.</p>
                                </body>
                                </html>
                        </xsl:template>
                    </xsl:stylesheet>
                    ";
    }
}
