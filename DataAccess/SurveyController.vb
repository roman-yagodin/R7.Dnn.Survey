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
' 4/15/2011 - Converted to WAP and Updated to DNN 5 Compliance by Robert J Collins 
' 4/15/2011 - Optional Interfaces for Import, Export, and Search moved to dedicated Business Layer by Robert J Collins 
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

    Public Class SurveyController

        Public Shared Function GetSurveys(ByVal ModuleId As Integer) As List(Of SurveyInfo)
            Dim SurveyInfolist As List(Of SurveyInfo) = New List(Of SurveyInfo)
            Using dr As IDataReader = DataProvider.Instance().GetSurveys(ModuleId)
                While dr.Read
                    Dim SurveyInfo As SurveyInfo = New SurveyInfo
                    SurveyInfo.SurveyId = Convert.ToInt32(dr("SurveyId"))
                    SurveyInfo.Question = Convert.ToString(dr("Question"))
                    SurveyInfo.OptionType = Convert.ToString(dr("OptionType"))
                    SurveyInfo.ViewOrder = Convert.ToInt32(ConvertNullInteger(dr("ViewOrder")))
                    SurveyInfo.CreatedByUser = Convert.ToInt32(dr("CreatedByUser"))
                    SurveyInfo.CreatedDate = Convert.ToDateTime(dr("CreatedDate"))
                    SurveyInfolist.Add(SurveyInfo)
                End While
            End Using
            Return SurveyInfolist
        End Function

        Public Shared Function GetSurvey(ByVal SurveyID As Integer, ByVal ModuleId As Integer) As SurveyInfo
            Dim SurveyInfo As SurveyInfo = New SurveyInfo
            Using dr As IDataReader = DataProvider.Instance().GetSurvey(SurveyID, ModuleId)
                While dr.Read
                    SurveyInfo.SurveyId = Convert.ToInt32(dr("SurveyId"))
                    SurveyInfo.ModuleId = Convert.ToInt32(dr("ModuleID"))
                    SurveyInfo.Question = Convert.ToString(dr("Question"))
                    SurveyInfo.OptionType = Convert.ToString(dr("OptionType"))
                    SurveyInfo.ViewOrder = Convert.ToInt32(ConvertNullInteger(dr("ViewOrder")))
                    SurveyInfo.Votes = Convert.ToInt32(ConvertNullInteger(dr("Votes")))
                    SurveyInfo.CreatedByUser = Convert.ToInt32(dr("CreatedByUser"))
                    SurveyInfo.CreatedDate = Convert.ToDateTime(dr("CreatedDate"))
                End While
            End Using
            Return SurveyInfo
        End Function

        Public Shared Sub DeleteSurvey(ByVal objSurvey As SurveyInfo)
            DataProvider.Instance().DeleteSurvey(objSurvey.SurveyId, objSurvey.ModuleId)
        End Sub

        Public Shared Function AddSurvey(ByVal objSurvey As SurveyInfo) As Integer
            Return CType(DataProvider.Instance().AddSurvey(objSurvey.ModuleId, objSurvey.Question, objSurvey.ViewOrder, objSurvey.OptionType, objSurvey.CreatedByUser), Integer)
        End Function

        Public Shared Sub UpdateSurvey(ByVal objSurvey As SurveyInfo)
            DataProvider.Instance().UpdateSurvey(objSurvey.SurveyId, objSurvey.Question, objSurvey.ViewOrder, objSurvey.OptionType, objSurvey.CreatedByUser, objSurvey.ModuleId)
        End Sub

#Region " Utility Functions "
        Public Shared Function ConvertNullInteger(ByVal Field As Object) As Integer
            If Field Is DBNull.Value Then
                Return 0
            Else
                Return Convert.ToInt32(Field)
            End If
        End Function
#End Region

    End Class
End Namespace

