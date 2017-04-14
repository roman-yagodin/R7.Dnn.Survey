'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2007
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
' 4/15/2011 - Initially Added by Robert J Collins 
'

Imports System
Imports System.Xml
Imports System.Xml.XPath
Imports System.Data
Imports System.Collections.Generic
Imports DotNetNuke
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Common.Utilities.XmlUtils

Namespace DotNetNuke.Modules.Survey

    Public Class SurveyBusinessController
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.ISearchable

        Dim sc As New SurveyController()

#Region "Optional Interfaces"

        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            Dim strXML As New StringBuilder()

            Dim settings As New XmlWriterSettings()
            settings.Indent = True
            settings.OmitXmlDeclaration = True

            Dim Writer As XmlWriter = XmlWriter.Create(strXML, settings)
            '----
            'Outer Loop - To build the Surveys
            Dim colSurveys As List(Of SurveyInfo) = sc.GetSurveys(ModuleID)
            If colSurveys.Count > 0 Then
                Writer.WriteStartElement("surveys")
                Dim SurveyInfo As SurveyInfo
                For Each SurveyInfo In colSurveys
                    Writer.WriteStartElement("survey")
                    Writer.WriteElementString("question", SurveyInfo.Question)
                    Writer.WriteElementString("vieworder", SurveyInfo.ViewOrder)
                    Writer.WriteElementString("createdbyuser", SurveyInfo.CreatedByUser)
                    Writer.WriteElementString("createddate", SurveyInfo.CreatedDate)
                    Writer.WriteElementString("optiontype", SurveyInfo.OptionType.ToString)

                    'Inner Loop - To build the Options for each Survey
                    Dim colSurveyOptions As List(Of SurveyOptionInfo) = SurveyOptionController.GetSurveyOptions(SurveyInfo.SurveyId)
                    If colSurveyOptions.Count > 0 Then
                        Writer.WriteStartElement("surveyoptions")
                        Dim SurveyOptionInfo As SurveyOptionInfo
                        For Each SurveyOptionInfo In colSurveyOptions
                            Writer.WriteStartElement("surveyoption")
                            Writer.WriteElementString("optionname", SurveyOptionInfo.OptionName)
                            Writer.WriteElementString("iscorrect", SurveyOptionInfo.IsCorrect)
                            Writer.WriteElementString("vieworder", SurveyOptionInfo.ViewOrder)
                            Writer.WriteEndElement()
                        Next ' Retrieve the next SurveyOption
                        Writer.WriteEndElement()
                    End If
                    Writer.WriteEndElement()

                Next
                Writer.WriteEndElement()
                Writer.Close()
            Else
                ' There is nothing to export
                Return String.Empty
            End If
            Return strXML.ToString()
        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements Entities.Modules.IPortable.ImportModule
            'Import the Surveys
            'Outer Loop - To insert the Surveys
            Dim intCurrentSurvey As Integer
            Dim xmlSurvey As XmlNode
            Dim xmlSurveys As XmlNode = Common.Globals.GetContent(Content, "surveys")

            For Each xmlSurvey In xmlSurveys

                Dim SurveyInfo As New SurveyInfo
                SurveyInfo.ModuleId = ModuleID
                SurveyInfo.Question = xmlSurvey.Item("question").InnerText
                SurveyInfo.ViewOrder = xmlSurvey.Item("vieworder").InnerText
                SurveyInfo.CreatedByUser = xmlSurvey.Item("createdbyuser").InnerText
                SurveyInfo.CreatedDate = xmlSurvey.Item("createddate").InnerText
                SurveyInfo.OptionType = xmlSurvey.Item("optiontype").InnerText

                'Add the Survey to the database
                intCurrentSurvey = sc.AddSurvey(SurveyInfo)

                'Inner Loop - To insert the Survey Options
                Dim xmlSurveyOption As XmlNode
                Dim xmlSurveyOptions As XmlNode = xmlSurvey.SelectSingleNode("surveyoptions")

                For Each xmlSurveyOption In xmlSurveyOptions
                    Dim SurveyOptionInfo As New SurveyOptionInfo
                    SurveyOptionInfo.SurveyId = intCurrentSurvey
                    SurveyOptionInfo.OptionName = xmlSurveyOption.Item("optionname").InnerText
                    SurveyOptionInfo.IsCorrect = xmlSurveyOption.Item("iscorrect").InnerText
                    SurveyOptionInfo.ViewOrder = xmlSurveyOption.Item("vieworder").InnerText

                    'Add the Survey to the database
                    SurveyOptionController.AddSurveyOption(SurveyOptionInfo)
                Next

            Next ' Retrieve the next Survey
        End Sub

        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) _
As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            ' Get the Surveys for this Module instance
            Dim colSurveys As List(Of SurveyInfo) = sc.GetSurveys(ModInfo.ModuleID)

            Dim SearchItemCollection As New SearchItemInfoCollection
            Dim SurveyInfo As SurveyInfo

            For Each SurveyInfo In colSurveys
                Dim SearchItem As SearchItemInfo
                SearchItem = New SearchItemInfo _
                (ModInfo.ModuleTitle & " - " & SurveyInfo.Question, _
                SurveyInfo.Question, _
                SurveyInfo.CreatedByUser, _
                SurveyInfo.CreatedDate, ModInfo.ModuleID, _
                SurveyInfo.SurveyId, _
                SurveyInfo.Question)
                SearchItemCollection.Add(SearchItem)
            Next

            Return SearchItemCollection
        End Function

#End Region

    End Class

End Namespace
