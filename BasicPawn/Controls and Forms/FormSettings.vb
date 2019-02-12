﻿'BasicPawn
'Copyright(C) 2018 TheTimocop

'This program Is free software: you can redistribute it And/Or modify
'it under the terms Of the GNU General Public License As published by
'the Free Software Foundation, either version 3 Of the License, Or
'(at your option) any later version.

'This program Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty Of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with this program. If Not, see < http: //www.gnu.org/licenses/>.

Public Class FormSettings
    Private g_mFormMain As FormMain
    Private g_iConfigType As ENUM_CONFIG_TYPE = ENUM_CONFIG_TYPE.ACTIVE

    Private g_lRestoreConfigs As New List(Of ClassConfigs.STRUC_CONFIG_ITEM)
    Private g_bRestoreConfigs As Boolean = False
    Private g_bIgnoreChange As Boolean = False
    Private g_bConfigSettingsChanged As Boolean = False
    Private g_bComboBoxIgnoreEvent As Boolean = False

    Private g_mListBoxConfigSelectedItem As Object = Nothing

    Private Shared bSuppressSyntaxVersionCheck As Boolean = False

    Enum ENUM_CONFIG_TYPE
        ALL
        ACTIVE
    End Enum

    Enum ENUM_PLUGIN_IMAGE_STATE
        ENABLED
        DISABLED
    End Enum

    Public Sub New(f As FormMain, iConfigType As ENUM_CONFIG_TYPE)
        g_mFormMain = f
        g_iConfigType = iConfigType
        g_bIgnoreChange = True

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call. 
        ImageList_Plugins.Images.Clear()
        ImageList_Plugins.Images.Add(CStr(ENUM_PLUGIN_IMAGE_STATE.ENABLED), My.Resources.netshell_1610_16x16)
        ImageList_Plugins.Images.Add(CStr(ENUM_PLUGIN_IMAGE_STATE.DISABLED), My.Resources.netshell_1608_16x16)

        Select Case (iConfigType)
            Case ENUM_CONFIG_TYPE.ALL
                TabPage_Configs.Text &= " (All Tabs)"

            Case Else
                TabPage_Configs.Text &= " (Active Tab)"
        End Select

        g_bComboBoxIgnoreEvent = True
        ComboBox_Language.Items.Clear()
        ComboBox_Language.Items.Add("Auto-detect")
        ComboBox_Language.Items.Add("SourcePawn")
        ComboBox_Language.Items.Add("AMX Mod X")
        ComboBox_Language.SelectedIndex = 0
        g_bComboBoxIgnoreEvent = False

        'SourceMod
        If (True) Then
            g_bComboBoxIgnoreEvent = True
            ComboBox_COOptimizationLevelSP.Items.Clear()
            ComboBox_COOptimizationLevelSP.Items.Add("Default")
            ComboBox_COOptimizationLevelSP.Items.Add("0 - No optimization")
            ComboBox_COOptimizationLevelSP.Items.Add("2 - Full optimizations")

            ComboBox_COVerbosityLevelSP.Items.Clear()
            ComboBox_COVerbosityLevelSP.Items.Add("Default")
            ComboBox_COVerbosityLevelSP.Items.Add("0 - Quiet")
            ComboBox_COVerbosityLevelSP.Items.Add("1 - Normal")
            ComboBox_COVerbosityLevelSP.Items.Add("2 - Verbose")

            ComboBox_COTreatWarningsAsErrorsSP.Items.Clear()
            ComboBox_COTreatWarningsAsErrorsSP.Items.Add("Default")
            ComboBox_COTreatWarningsAsErrorsSP.Items.Add("True")
            ComboBox_COTreatWarningsAsErrorsSP.Items.Add("False")
            g_bComboBoxIgnoreEvent = False
        End If

        'AMX Mod X
        If (True) Then
            g_bComboBoxIgnoreEvent = True
            ComboBox_COSymbolicInformationAMXX.Items.Clear()
            ComboBox_COSymbolicInformationAMXX.Items.Add("Default")
            ComboBox_COSymbolicInformationAMXX.Items.Add("0 - No symbolic information, no run-time checks")
            ComboBox_COSymbolicInformationAMXX.Items.Add("1 - Run-time checks, no symbolic information")
            ComboBox_COSymbolicInformationAMXX.Items.Add("2 - Full debug information And dynamic checking")
            ComboBox_COSymbolicInformationAMXX.Items.Add("3 - Full debug information, dynamic checking, no optimization")

            ComboBox_COVerbosityLevelAMXX.Items.Clear()
            ComboBox_COVerbosityLevelAMXX.Items.Add("Default")
            ComboBox_COVerbosityLevelAMXX.Items.Add("0 - Quiet")
            ComboBox_COVerbosityLevelAMXX.Items.Add("1 - Normal")
            ComboBox_COVerbosityLevelAMXX.Items.Add("2 - Verbose")

            ComboBox_COTreatWarningsAsErrorsAMXX.Items.Clear()
            ComboBox_COTreatWarningsAsErrorsAMXX.Items.Add("Default")
            ComboBox_COTreatWarningsAsErrorsAMXX.Items.Add("True")
            ComboBox_COTreatWarningsAsErrorsAMXX.Items.Add("False")
            g_bComboBoxIgnoreEvent = False
        End If

        If (ComboBox_Language.Items.Count <> [Enum].GetNames(GetType(ClassConfigs.STRUC_CONFIG_ITEM.ENUM_LANGUAGE_DETECT_TYPE)).Length) Then
            Throw New ArgumentException("ComboBox_Language range")
        End If

        Me.Size = Me.MinimumSize

        ClassTools.ClassForms.SetDoubleBufferingAllChilds(Me, True)
        ClassTools.ClassForms.SetDoubleBufferingUnmanagedAllChilds(Me, True)

        g_bIgnoreChange = False
        m_ConfigSettingsChanged = False
    End Sub

#Region "Load/Save/General"
    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'List all configs
        UpdateConfigListBox()

        'Update log button text
        UpdateErrorLogSize()

        'Get all settings
        ClassSettings.LoadSettings()
        ClassConfigs.ClassKnownConfigs.LoadKnownConfigs()

        'General
        CheckBox_AlwaysNewInstance.Checked = ClassSettings.g_iSettingsAlwaysOpenNewInstance
        CheckBox_AutoShowStartPage.Checked = ClassSettings.g_iSettingsAutoShowStartPage
        CheckBox_AutoOpenProjectFiles.Checked = ClassSettings.g_iSettingsAutoOpenProjectFiles
        CheckBox_AssociateSourcePawn.Checked = ClassSettings.g_iSettingsAssociateSourcePawn
        CheckBox_AssociateAmxMod.Checked = ClassSettings.g_iSettingsAssociateAmxModX
        CheckBox_AssociateIncludes.Checked = ClassSettings.g_iSettingsAssociateIncludes
        CheckBox_AutoHoverScroll.Checked = ClassSettings.g_iSettingsAutoHoverScroll
        'Text Editor
        Label_Font.Text = New FontConverter().ConvertToInvariantString(ClassSettings.g_iSettingsTextEditorFont)
        CheckBox_InvertedColors.Checked = ClassSettings.g_iSettingsInvertColors
        CheckBox_TabsToSpace.Checked = (ClassSettings.g_iSettingsTabsToSpaces > 0)
        NumericUpDown_TabsToSpaces.Value = If(ClassSettings.g_iSettingsTabsToSpaces > 0, ClassSettings.g_iSettingsTabsToSpaces, 4)
        TextBox_CustomSyntax.Text = ClassSettings.g_sSettingsSyntaxHighlightingPath
        CheckBox_RememberFolds.Checked = ClassSettings.g_bSettingsRememberFoldings
        NumericUpDown_ThreadUpdateRate.Value = ClassSettings.g_iSettingsThreadUpdateRate
        'Syntax Highligting
        CheckBox_DoubleClickMark.Checked = ClassSettings.g_iSettingsDoubleClickMark
        CheckBox_AutoMark.Checked = ClassSettings.g_iSettingsAutoMark
        'Autocomplete
        CheckBox_AlwaysLoadDefaultIncludes.Checked = ClassSettings.g_iSettingsAlwaysLoadDefaultIncludes
        CheckBox_OnScreenIntelliSense.Checked = ClassSettings.g_iSettingsEnableToolTip
        CheckBox_CommentsMethodIntelliSense.Checked = ClassSettings.g_iSettingsToolTipMethodComments
        CheckBox_CommentsAutocompleteIntelliSense.Checked = ClassSettings.g_iSettingsToolTipAutocompleteComments
        CheckBox_WindowsToolTipPopup.Checked = ClassSettings.g_iSettingsUseWindowsToolTip
        CheckBox_WindowsToolTipAnimations.Checked = ClassSettings.g_iSettingsUseWindowsToolTipAnimations
        CheckBox_WindowsToolTipNewlineMethods.Checked = ClassSettings.g_iSettingsUseWindowsToolTipNewlineMethods
        CheckBox_WindowsToolTipDisplayTop.Checked = ClassSettings.g_iSettingsUseWindowsToolTipDisplayTop
        CheckBox_FullAutcompleteMethods.Checked = ClassSettings.g_iSettingsFullMethodAutocomplete
        CheckBox_FullAutocompleteReTagging.Checked = ClassSettings.g_iSettingsFullEnumAutocomplete
        CheckBox_CaseSensitive.Checked = ClassSettings.g_iSettingsAutocompleteCaseSensitive

        Select Case (ClassSettings.g_iSettingsAutocompleteVarParseType)
            Case ClassSettings.ENUM_VAR_PARSE_TYPE.ALL
                RadioButton_VarParseAll.Checked = True
            Case ClassSettings.ENUM_VAR_PARSE_TYPE.TAB_AND_INC
                RadioButton_VarParseTabInc.Checked = True
            Case Else
                RadioButton_VarParseTab.Checked = True
        End Select

        CheckBox_VarAutocompleteShowObjectBrowser.Checked = ClassSettings.g_iSettingsObjectBrowserShowVariables
        CheckBox_SwitchTabToAutocomplete.Checked = ClassSettings.g_iSettingsSwitchTabToAutocomplete
        CheckBox_OnlyUpdateSyntaxWhenFocused.Checked = ClassSettings.g_iSettingsOnlyUpdateSyntaxWhenFocused
        CheckBox_AutoCloseBrackets.Checked = ClassSettings.g_iSettingsAutoCloseBrackets
        CheckBox_AutoCloseStrings.Checked = ClassSettings.g_iSettingsAutoCloseStrings
        CheckBox_AutoIndentBrackets.Checked = ClassSettings.g_iSettingsAutoIndentBrackets

        'Get restore-point configs 
        For Each mConfig As ClassConfigs.STRUC_CONFIG_ITEM In ClassConfigs.GetConfigs()
            g_lRestoreConfigs.Add(mConfig)
        Next
        g_bRestoreConfigs = True

        'Get current config 
        'NOTE: Only get the first 
        Dim mCurrentConfig = g_mFormMain.g_ClassTabControl.m_ActiveTab.m_ActiveConfig

        TextBox_ConfigName.Text = mCurrentConfig.GetName

        Dim i As Integer = ListBox_Configs.FindStringExact(mCurrentConfig.GetName)
        If (i > -1) Then
            ListBox_Configs.SetSelected(i, True)
        End If

        If (Not mCurrentConfig.ConfigExist AndAlso Not mCurrentConfig.IsDefault) Then
            MessageBox.Show("Current config not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If


        'List plugins
        UpdatePluginsListView()

        'Fill DatabaseViewer
        DatabaseListBox_Database.FillFromDatabase()

        ClassControlStyle.UpdateControls(Me)

        'Load last window info
        ClassSettings.LoadWindowInfo(Me)
    End Sub

    Private Sub Button_Apply_Click(sender As Object, e As EventArgs) Handles Button_Apply.Click
        g_bRestoreConfigs = False

        PromptSaveSelectedConfig()

        If (g_mListBoxConfigSelectedItem IsNot Nothing) Then
            Dim sName As String = g_mListBoxConfigSelectedItem.ToString
            Dim mConfig = ClassConfigs.LoadConfig(sName)

            Select Case (g_iConfigType)
                Case ENUM_CONFIG_TYPE.ALL
                    For i = 0 To g_mFormMain.g_ClassTabControl.m_TabsCount - 1
                        g_mFormMain.g_ClassTabControl.m_Tab(i).m_ActiveConfig = mConfig

                        If (Not g_mFormMain.g_ClassTabControl.m_Tab(i).m_IsUnsaved AndAlso Not g_mFormMain.g_ClassTabControl.m_Tab(i).m_InvalidFile) Then
                            ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(g_mFormMain.g_ClassTabControl.m_Tab(i).m_File) = mConfig

                            'Only assign optimal config if a config has been found.
                            Dim j As ClassConfigs.ENUM_OPTIMAL_CONFIG
                            Dim mOptimalConfig = ClassConfigs.FindOptimalConfigForFile(g_mFormMain.g_ClassTabControl.m_Tab(i).m_File, False, j)
                            If (j <> ClassConfigs.ENUM_OPTIMAL_CONFIG.NONE) Then
                                g_mFormMain.g_ClassTabControl.m_Tab(i).m_ActiveConfig = mOptimalConfig
                                ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(g_mFormMain.g_ClassTabControl.m_Tab(i).m_File) = mOptimalConfig
                            End If
                        End If
                    Next

                Case Else
                    g_mFormMain.g_ClassTabControl.m_ActiveTab.m_ActiveConfig = mConfig

                    If (Not g_mFormMain.g_ClassTabControl.m_ActiveTab.m_IsUnsaved AndAlso Not g_mFormMain.g_ClassTabControl.m_ActiveTab.m_InvalidFile) Then
                        ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(g_mFormMain.g_ClassTabControl.m_ActiveTab.m_File) = mConfig

                        'Only assign optimal config if a config has been found.
                        Dim j As ClassConfigs.ENUM_OPTIMAL_CONFIG
                        Dim mOptimalConfig = ClassConfigs.FindOptimalConfigForFile(g_mFormMain.g_ClassTabControl.m_ActiveTab.m_File, False, j)
                        If (j <> ClassConfigs.ENUM_OPTIMAL_CONFIG.NONE) Then
                            g_mFormMain.g_ClassTabControl.m_ActiveTab.m_ActiveConfig = mOptimalConfig
                            ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(g_mFormMain.g_ClassTabControl.m_ActiveTab.m_File) = mOptimalConfig
                        End If
                    End If
            End Select
        End If

        'Cleanup invalid files from known configs
        Dim lConfigNames As New List(Of String)
        For Each mItem In ClassConfigs.GetConfigs()
            lConfigNames.Add(mItem.GetName)
        Next

        For Each mKnownConfig In ClassConfigs.ClassKnownConfigs.GetKnownConfigs
            If (Not lConfigNames.Contains(mKnownConfig.sConfigName)) Then
                ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(mKnownConfig.sFile) = Nothing
                Continue For
            End If

            If (Not IO.File.Exists(mKnownConfig.sFile)) Then
                ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(mKnownConfig.sFile) = Nothing
                Continue For
            End If
        Next

        'General
        ClassSettings.g_iSettingsAlwaysOpenNewInstance = CheckBox_AlwaysNewInstance.Checked
        ClassSettings.g_iSettingsAutoShowStartPage = CheckBox_AutoShowStartPage.Checked
        ClassSettings.g_iSettingsAutoOpenProjectFiles = CheckBox_AutoOpenProjectFiles.Checked
        ClassSettings.g_iSettingsAssociateSourcePawn = CheckBox_AssociateSourcePawn.Checked
        ClassSettings.g_iSettingsAssociateAmxModX = CheckBox_AssociateAmxMod.Checked
        ClassSettings.g_iSettingsAssociateIncludes = CheckBox_AssociateIncludes.Checked
        ClassSettings.g_iSettingsAutoHoverScroll = CheckBox_AutoHoverScroll.Checked
        'Text Editor
        ClassSettings.g_iSettingsTextEditorFont = CType(New FontConverter().ConvertFromInvariantString(Label_Font.Text), Font)
        ClassSettings.g_iSettingsInvertColors = CheckBox_InvertedColors.Checked
        ClassSettings.g_iSettingsTabsToSpaces = CInt(If(CheckBox_TabsToSpace.Checked, NumericUpDown_TabsToSpaces.Value, 0))
        ClassSettings.g_sSettingsSyntaxHighlightingPath = TextBox_CustomSyntax.Text
        ClassSettings.g_bSettingsRememberFoldings = CheckBox_RememberFolds.Checked
        ClassSettings.g_iSettingsThreadUpdateRate = CInt(NumericUpDown_ThreadUpdateRate.Value)
        'Syntax Highligting
        ClassSettings.g_iSettingsDoubleClickMark = CheckBox_DoubleClickMark.Checked
        ClassSettings.g_iSettingsAutoMark = CheckBox_AutoMark.Checked
        'Autocomplete
        ClassSettings.g_iSettingsAlwaysLoadDefaultIncludes = CheckBox_AlwaysLoadDefaultIncludes.Checked
        ClassSettings.g_iSettingsEnableToolTip = CheckBox_OnScreenIntelliSense.Checked
        ClassSettings.g_iSettingsToolTipMethodComments = CheckBox_CommentsMethodIntelliSense.Checked
        ClassSettings.g_iSettingsToolTipAutocompleteComments = CheckBox_CommentsAutocompleteIntelliSense.Checked
        ClassSettings.g_iSettingsUseWindowsToolTip = CheckBox_WindowsToolTipPopup.Checked
        ClassSettings.g_iSettingsUseWindowsToolTipAnimations = CheckBox_WindowsToolTipAnimations.Checked
        ClassSettings.g_iSettingsUseWindowsToolTipNewlineMethods = CheckBox_WindowsToolTipNewlineMethods.Checked
        ClassSettings.g_iSettingsUseWindowsToolTipDisplayTop = CheckBox_WindowsToolTipDisplayTop.Checked
        ClassSettings.g_iSettingsFullMethodAutocomplete = CheckBox_FullAutcompleteMethods.Checked
        ClassSettings.g_iSettingsFullEnumAutocomplete = CheckBox_FullAutocompleteReTagging.Checked
        ClassSettings.g_iSettingsAutocompleteCaseSensitive = CheckBox_CaseSensitive.Checked

        Select Case (True)
            Case RadioButton_VarParseAll.Checked
                ClassSettings.g_iSettingsAutocompleteVarParseType = ClassSettings.ENUM_VAR_PARSE_TYPE.ALL
            Case RadioButton_VarParseTabInc.Checked
                ClassSettings.g_iSettingsAutocompleteVarParseType = ClassSettings.ENUM_VAR_PARSE_TYPE.TAB_AND_INC
            Case Else
                ClassSettings.g_iSettingsAutocompleteVarParseType = ClassSettings.ENUM_VAR_PARSE_TYPE.TAB
        End Select

        ClassSettings.g_iSettingsObjectBrowserShowVariables = CheckBox_VarAutocompleteShowObjectBrowser.Checked
        ClassSettings.g_iSettingsSwitchTabToAutocomplete = CheckBox_SwitchTabToAutocomplete.Checked
        ClassSettings.g_iSettingsOnlyUpdateSyntaxWhenFocused = CheckBox_OnlyUpdateSyntaxWhenFocused.Checked
        ClassSettings.g_iSettingsAutoCloseBrackets = CheckBox_AutoCloseBrackets.Checked
        ClassSettings.g_iSettingsAutoCloseStrings = CheckBox_AutoCloseStrings.Checked
        ClassSettings.g_iSettingsAutoIndentBrackets = CheckBox_AutoIndentBrackets.Checked

        ClassSettings.SaveSettings()
        ClassConfigs.ClassKnownConfigs.SaveKnownConfigs()

        g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnSettingsChanged())

        Me.Close()
    End Sub

    Private Sub Button_Cancel_Click(sender As Object, e As EventArgs) Handles Button_Cancel.Click
        Me.Close()
    End Sub

    Private Sub FormSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (g_bRestoreConfigs) Then
            For Each mConfig As ClassConfigs.STRUC_CONFIG_ITEM In ClassConfigs.GetConfigs()
                mConfig.RemoveConfig()
            Next

            For Each mConfig As ClassConfigs.STRUC_CONFIG_ITEM In g_lRestoreConfigs
                mConfig.SaveConfig()
            Next
        End If

        'Check default config paths collisions
        If (True) Then
            Dim mConfigs = ClassConfigs.GetConfigs

            For Each mConfig In mConfigs
                Dim sDefaultConfigPaths As String() = mConfig.g_sDefaultPaths.Split(";"c)

                For Each mCompConfig In mConfigs
                    'Ignore same config
                    If (mConfig.GetName = mCompConfig.GetName) Then
                        Continue For
                    End If

                    Dim sCompDefaultConfigPaths As String() = mCompConfig.g_sDefaultPaths.Split(";"c)

                    For Each sPath As String In sDefaultConfigPaths
                        If (String.IsNullOrEmpty(sPath)) Then
                            Continue For
                        End If

                        For Each sCompPath As String In sCompDefaultConfigPaths
                            If (String.IsNullOrEmpty(sCompPath)) Then
                                Continue For
                            End If

                            If (sPath.ToLower.StartsWith(sCompPath.ToLower)) Then
                                With New Text.StringBuilder()
                                    .AppendLine("Some default config paths collide with other configs!")
                                    .AppendLine()
                                    .AppendLine()
                                    .AppendFormat("Config '{0}' collides with '{1}'.", mConfig.GetName, mCompConfig.GetName).AppendLine()

                                    MessageBox.Show(.ToString, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                End With
                            End If
                        Next
                    Next
                Next
            Next
        End If

        'Check for outdated syntax highlight files
        If (True) Then
            If (Not bSuppressSyntaxVersionCheck) Then
                Dim mCurrentVersion As New Version
                Dim mSyntaxVersion As New Version
                If (g_mFormMain.g_ClassSyntaxTools.g_ClassSyntaxHighlighting.CheckSyntaxVersion(mCurrentVersion, mSyntaxVersion)) Then
                    With New Text.StringBuilder()
                        .AppendLine("Your custom syntax highlighting file seems to be out-of-date.")
                        .AppendLine("Do you want to open the download page now?")
                        .AppendLine()
                        .AppendFormat("Your version is v{0}. Required version is v{1}.", mSyntaxVersion.ToString, mCurrentVersion.ToString).AppendLine()
                        .AppendLine()
                        .AppendLine("Click YES to go to the BasicPawn syntax download page.")
                        .AppendLine("Click CANCEL to suppress this warning.")

                        Select Case (MessageBox.Show(.ToString, "Syntax out-of-date", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                            Case DialogResult.Yes
                                Try
                                    Process.Start("https://github.com/Timocop/BasicPawn/tree/master/Custom%20Syntax%20Styles")
                                Catch ex As Exception
                                    ClassExceptionLog.WriteToLogMessageBox(ex)
                                End Try

                            Case DialogResult.Cancel
                                bSuppressSyntaxVersionCheck = True
                        End Select
                    End With
                End If
            End If
        End If

        'Save window info
        ClassSettings.SaveWindowInfo(Me)
    End Sub

    Private Sub Button_ClearErrorLog_Click(sender As Object, e As EventArgs) Handles Button_ClearErrorLog.Click
        Try
            If (IO.File.Exists(ClassExceptionLog.g_sLogName)) Then
                IO.File.Delete(ClassExceptionLog.g_sLogName)
            End If
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        Finally
            UpdateErrorLogSize()
        End Try
    End Sub

    Private Sub Button_ViewErrorLog_Click(sender As Object, e As EventArgs) Handles Button_ViewErrorLog.Click
        Try
            If (Not IO.File.Exists(ClassExceptionLog.g_sLogName)) Then
                MessageBox.Show("Log file does not exist", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Process.Start("notepad", String.Format("""{0}""", ClassExceptionLog.g_sLogName))
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        Finally
            UpdateErrorLogSize()
        End Try
    End Sub

    Private Sub UpdateErrorLogSize()
        Try
            Dim mLogInfo As New IO.FileInfo(ClassExceptionLog.g_sLogName)

            If (Not mLogInfo.Exists) Then
                Button_ClearErrorLog.Text = "Clear log (Empty)"
                Return
            End If

            Button_ClearErrorLog.Text = String.Format("Clear log ({0})", ClassTools.ClassStrings.FormatBytes(mLogInfo.Length))
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub
#End Region

#Region "Settings"
    Private Sub Button_Font_Click(sender As Object, e As EventArgs) Handles Button_Font.Click
        Using i As New FontDialog()
            i.Font = CType(New FontConverter().ConvertFromInvariantString(Label_Font.Text), Font)

            If (i.ShowDialog = DialogResult.OK) Then
                Label_Font.Text = New FontConverter().ConvertToInvariantString(i.Font)
            End If
        End Using
    End Sub

    Private Sub Button_CustomSyntax_Click(sender As Object, e As EventArgs) Handles Button_CustomSyntax.Click
        Using i As New OpenFileDialog
            i.Filter = "Syntax highlighting XSHD file|*.xml"
            i.FileName = TextBox_CustomSyntax.Text

            If (i.ShowDialog = DialogResult.OK) Then
                TextBox_CustomSyntax.Text = i.FileName
            End If
        End Using
    End Sub

    Private Sub LinkLabel_DefaultSyntax_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel_DefaultSyntax.LinkClicked
        TextBox_CustomSyntax.Text = ""
    End Sub

    Private Sub LinkLabel_MoreStyles_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel_MoreStyles.LinkClicked
        Try
            Process.Start("https://github.com/Timocop/BasicPawn/tree/master/Custom%20Syntax%20Styles")
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub
#End Region

#Region "Configs"
    Private Property m_ConfigSettingsChanged As Boolean
        Get
            Return g_bConfigSettingsChanged
        End Get
        Set(value As Boolean)
            If (g_bIgnoreChange) Then
                Return
            End If

            g_bConfigSettingsChanged = value

            GroupBox_ConfigSettings.Text = GroupBox_ConfigSettings.Text.TrimEnd("*"c) & If(g_bConfigSettingsChanged, "*"c, "")
            Button_SaveConfig.Font = New Font(Button_SaveConfig.Font, If(g_bConfigSettingsChanged, FontStyle.Bold, FontStyle.Regular))
        End Set
    End Property

    Private Sub Button_ConfigAdd_Click(sender As Object, e As EventArgs) Handles Button_ConfigAdd.Click
        Dim sNewName As String = TextBox_ConfigName.Text

        If (String.IsNullOrEmpty(sNewName) OrElse sNewName = ClassConfigs.m_DefaultConfig.GetName OrElse sNewName.IndexOfAny(IO.Path.GetInvalidFileNameChars) > -1 OrElse sNewName.IndexOfAny(IO.Path.GetInvalidPathChars) > -1) Then
            MessageBox.Show("Invalid config name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If (ClassConfigs.LoadConfig(sNewName) IsNot Nothing) Then
            MessageBox.Show("This config name is already used!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ListBox_Configs.Items.Remove(sNewName)
        ListBox_Configs.Items.Add(sNewName)
        ClassConfigs.SaveConfig(New ClassConfigs.STRUC_CONFIG_ITEM(sNewName))

        MarkChanged()

        g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnConfigChanged())
    End Sub

    Private Sub Button_ConfigRemove_Click(sender As Object, e As EventArgs) Handles Button_ConfigRemove.Click
        Try
            Dim sName As String = TextBox_ConfigName.Text

            If (ClassConfigs.RemoveConfig(sName)) Then
                ListBox_Configs.Items.Remove(sName)

                MarkChanged()

                g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnConfigChanged())
            End If
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Updates the configs in the ListBox
    ''' </summary>
    Private Sub UpdateConfigListBox()
        ListBox_Configs.BeginUpdate()
        ListBox_Configs.Items.Clear()

        For Each mConfig As ClassConfigs.STRUC_CONFIG_ITEM In ClassConfigs.GetConfigs()
            ListBox_Configs.Items.Add(mConfig.GetName)
        Next

        ListBox_Configs.EndUpdate()
    End Sub

    Private Sub ListBox_Configs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox_Configs.SelectedIndexChanged
        Try
            PromptSaveSelectedConfig()

            If (ListBox_Configs.SelectedItems.Count < 1) Then
                g_mListBoxConfigSelectedItem = Nothing

                GroupBox_ConfigSettings.Enabled = False
                Return
            Else
                g_mListBoxConfigSelectedItem = ListBox_Configs.SelectedItems(0)
            End If

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString

            If (sName = ClassConfigs.m_DefaultConfig.GetName) Then
                g_bIgnoreChange = True

                Button_SaveConfig.Enabled = False
                GroupBox_ConfigSettings.Enabled = False
                GroupBox_ConfigSettings.Visible = False

                'Fill controls
                If (True) Then
                    'General
                    TextBox_ConfigName.Text = sName
                    RadioButton_ConfigSettingAutomatic.Checked = True
                    TextBox_CompilerPath.Text = ""
                    TextBox_IncludeFolder.Text = ""
                    TextBox_OutputFolder.Text = ""
                    CheckBox_ConfigIsDefault.Checked = False
                    TextBox_AutoAssignPaths.Text = ""
                    ComboBox_Language.SelectedIndex = 0
                    If (True) Then
                        'Compiler Options
                        '   (SourcePawn)
                        ComboBox_COOptimizationLevelSP.SelectedIndex = 0
                        ComboBox_COVerbosityLevelSP.SelectedIndex = 0
                        ComboBox_COTreatWarningsAsErrorsSP.SelectedIndex = 0
                        TextBoxEx_COIgnoredWarningsSP.Text = ""
                        TextBoxEx_CODefineConstantsSP.Text = ""
                        TextBoxEx_COIgnoredWarningsSP.ShowWatermark()
                        TextBoxEx_CODefineConstantsSP.ShowWatermark()
                        '   (AMX Mod X) 
                        ComboBox_COVerbosityLevelAMXX.SelectedIndex = 0
                        ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndex = 0
                        ComboBox_COSymbolicInformationAMXX.SelectedIndex = 0
                        TextBoxEx_COIgnoredWarningsAMXX.Text = ""
                        TextBoxEx_CODefineConstantsAMXX.Text = ""
                        TextBoxEx_COIgnoredWarningsAMXX.ShowWatermark()
                        TextBoxEx_CODefineConstantsAMXX.ShowWatermark()
                    End If
                    ListView_KnownFiles.Items.Clear()

                    'Debugging
                    TextBox_ClientFolder.Text = ""
                    TextBox_ServerFolder.Text = ""
                    TextBox_SourceModFolder.Text = ""

                    'Misc
                    TextBox_Shell.Text = ""
                End If

                g_bIgnoreChange = False
                m_ConfigSettingsChanged = False

                'ResetChanged()
                Return
            End If

            g_bIgnoreChange = True
            Button_SaveConfig.Enabled = True
            GroupBox_ConfigSettings.Enabled = True
            GroupBox_ConfigSettings.Visible = True

            'Fill controls
            If (True) Then
                'General
                TextBox_ConfigName.Text = sName
                RadioButton_ConfigSettingAutomatic.Checked = True
                TextBox_CompilerPath.Text = ""
                TextBox_IncludeFolder.Text = ""
                TextBox_OutputFolder.Text = ""
                CheckBox_ConfigIsDefault.Checked = False
                TextBox_AutoAssignPaths.Text = ""
                ComboBox_Language.SelectedIndex = 0
                If (True) Then
                    'Compiler Options
                    '   (SourcePawn)
                    ComboBox_COOptimizationLevelSP.SelectedIndex = 0
                    ComboBox_COVerbosityLevelSP.SelectedIndex = 0
                    ComboBox_COTreatWarningsAsErrorsSP.SelectedIndex = 0
                    TextBoxEx_COIgnoredWarningsSP.Text = ""
                    TextBoxEx_CODefineConstantsSP.Text = ""
                    TextBoxEx_COIgnoredWarningsSP.ShowWatermark()
                    TextBoxEx_CODefineConstantsSP.ShowWatermark()
                    '   (AMX Mod X) 
                    ComboBox_COVerbosityLevelAMXX.SelectedIndex = 0
                    ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndex = 0
                    ComboBox_COSymbolicInformationAMXX.SelectedIndex = 0
                    TextBoxEx_COIgnoredWarningsAMXX.Text = ""
                    TextBoxEx_CODefineConstantsAMXX.Text = ""
                    TextBoxEx_COIgnoredWarningsAMXX.ShowWatermark()
                    TextBoxEx_CODefineConstantsAMXX.ShowWatermark()
                End If
                ListView_KnownFiles.Items.Clear()

                'Debugging
                TextBox_ClientFolder.Text = ""
                TextBox_ServerFolder.Text = ""
                TextBox_SourceModFolder.Text = ""

                'Misc
                TextBox_Shell.Text = ""
            End If

            g_bIgnoreChange = False
            m_ConfigSettingsChanged = False

            Dim mConfig As ClassConfigs.STRUC_CONFIG_ITEM = ClassConfigs.LoadConfig(sName)

            If (mConfig Is Nothing) Then
                MessageBox.Show("Current config not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            g_bIgnoreChange = True

            'Fill controls
            If (True) Then
                'General
                RadioButton_ConfigSettingAutomatic.Checked = True
                RadioButton_ConfigSettingManual.Checked = (mConfig.g_iCompilingType = ClassSettings.ENUM_COMPILING_TYPE.CONFIG)
                TextBox_CompilerPath.Text = mConfig.g_sCompilerPath
                TextBox_IncludeFolder.Text = mConfig.g_sIncludeFolders
                TextBox_OutputFolder.Text = mConfig.g_sOutputFolder
                CheckBox_ConfigIsDefault.Checked = mConfig.g_bAutoload
                TextBox_AutoAssignPaths.Text = mConfig.g_sDefaultPaths
                ComboBox_Language.SelectedIndex = mConfig.g_iLanguage

                'Compiler Options
                '   SourcePawn
                If (True) Then
                    Select Case (mConfig.g_mCompilerOptionsSP.g_iOptimizationLevel)
                        Case 0
                            ComboBox_COOptimizationLevelSP.SelectedIndex = 1
                        Case 2
                            ComboBox_COOptimizationLevelSP.SelectedIndex = 2
                    End Select

                    Select Case (mConfig.g_mCompilerOptionsSP.g_iVerbosityLevel)
                        Case 0
                            ComboBox_COVerbosityLevelSP.SelectedIndex = 1
                        Case 1
                            ComboBox_COVerbosityLevelSP.SelectedIndex = 2
                        Case 2
                            ComboBox_COVerbosityLevelSP.SelectedIndex = 3
                        Case 3
                            ComboBox_COVerbosityLevelSP.SelectedIndex = 4
                    End Select

                    Select Case (mConfig.g_mCompilerOptionsSP.g_iTreatWarningsAsErrors)
                        Case 1
                            ComboBox_COTreatWarningsAsErrorsSP.SelectedIndex = 1
                        Case 0
                            ComboBox_COTreatWarningsAsErrorsSP.SelectedIndex = 2
                    End Select

                    TextBoxEx_COIgnoredWarningsSP.Text = ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.IgnoredWarningsToString(mConfig.g_mCompilerOptionsSP.g_lIgnoredWarnings)
                    TextBoxEx_CODefineConstantsSP.Text = ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.DefineConstantsToString(mConfig.g_mCompilerOptionsSP.g_mDefineConstants)
                    TextBoxEx_COIgnoredWarningsSP.ShowWatermark()
                    TextBoxEx_CODefineConstantsSP.ShowWatermark()
                End If
                '   AMX Mod X
                If (True) Then
                    Select Case (mConfig.g_mCompilerOptionsAMXX.g_iSymbolicInformation)
                        Case 0
                            ComboBox_COSymbolicInformationAMXX.SelectedIndex = 1
                        Case 1
                            ComboBox_COSymbolicInformationAMXX.SelectedIndex = 2
                        Case 2
                            ComboBox_COSymbolicInformationAMXX.SelectedIndex = 3
                        Case 3
                            ComboBox_COSymbolicInformationAMXX.SelectedIndex = 4
                    End Select

                    Select Case (mConfig.g_mCompilerOptionsAMXX.g_iVerbosityLevel)
                        Case 0
                            ComboBox_COVerbosityLevelAMXX.SelectedIndex = 1
                        Case 1
                            ComboBox_COVerbosityLevelAMXX.SelectedIndex = 2
                        Case 2
                            ComboBox_COVerbosityLevelAMXX.SelectedIndex = 3
                        Case 3
                            ComboBox_COVerbosityLevelAMXX.SelectedIndex = 4
                    End Select

                    Select Case (mConfig.g_mCompilerOptionsAMXX.g_iTreatWarningsAsErrors)
                        Case 1
                            ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndex = 1
                        Case 0
                            ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndex = 2
                    End Select

                    TextBoxEx_COIgnoredWarningsAMXX.Text = ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.IgnoredWarningsToString(mConfig.g_mCompilerOptionsAMXX.g_lIgnoredWarnings)
                    TextBoxEx_CODefineConstantsAMXX.Text = ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.DefineConstantsToString(mConfig.g_mCompilerOptionsAMXX.g_mDefineConstants)
                    TextBoxEx_COIgnoredWarningsAMXX.ShowWatermark()
                    TextBoxEx_CODefineConstantsAMXX.ShowWatermark()
                End If
                RefreshKnownFilesListBox()

                'Debugging
                TextBox_ClientFolder.Text = mConfig.g_sDebugClientFolder
                TextBox_ServerFolder.Text = mConfig.g_sDebugServerFolder
                TextBox_SourceModFolder.Text = mConfig.g_sDebugSourceModFolder

                'Misc
                TextBox_Shell.Text = mConfig.g_sExecuteShell
            End If

            g_bIgnoreChange = False
            m_ConfigSettingsChanged = False

            'ResetChanged()
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub ListBox_Configs_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox_Configs.MouseDoubleClick
        Button_Apply.PerformClick()
    End Sub

    Private Sub Button_Compiler_Click(sender As Object, e As EventArgs) Handles Button_Compiler.Click
        Using i As New OpenFileDialog
            i.Filter = "SourcePawn Compiler|spcomp.exe|AMX Mod X Compiler|amxxpc.exe|Small Compiler|sc.exe|Pawn Compiler|pawncc.exe|Executables|*.exe"
            i.FileName = TextBox_CompilerPath.Text

            If (i.ShowDialog = DialogResult.OK) Then
                TextBox_CompilerPath.Text = i.FileName
            End If
        End Using
    End Sub

    Private Sub Button_OutputFolder_Click(sender As Object, e As EventArgs) Handles Button_OutputFolder.Click
        Using i As New FolderBrowserDialog
            If (String.IsNullOrEmpty(TextBox_OutputFolder.Text) AndAlso Not String.IsNullOrEmpty(TextBox_CompilerPath.Text) AndAlso IO.File.Exists(TextBox_CompilerPath.Text)) Then
                i.SelectedPath = IO.Path.GetDirectoryName(TextBox_CompilerPath.Text)
            Else
                i.SelectedPath = TextBox_OutputFolder.Text
            End If

            If (i.ShowDialog = DialogResult.OK) Then
                TextBox_OutputFolder.Text = i.SelectedPath
            End If
        End Using
    End Sub

    Private Sub Button_IncludeFolder_Click(sender As Object, e As EventArgs) Handles Button_IncludeFolder.Click
        Using i As New FolderBrowserDialog
            If (String.IsNullOrEmpty(TextBox_IncludeFolder.Text) AndAlso Not String.IsNullOrEmpty(TextBox_CompilerPath.Text) AndAlso IO.File.Exists(TextBox_CompilerPath.Text)) Then
                i.SelectedPath = IO.Path.GetDirectoryName(TextBox_CompilerPath.Text)
            Else
                i.SelectedPath = TextBox_IncludeFolder.Text.Split(";"c)(0)
            End If

            If (i.ShowDialog = DialogResult.OK) Then
                If (String.IsNullOrEmpty(TextBox_IncludeFolder.Text)) Then
                    TextBox_IncludeFolder.Text = i.SelectedPath
                Else
                    Select Case MessageBox.Show("Replace already existing paths with this one? Otherwise the selected path will be addded to other already existing paths.", "Replace or add paths", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        Case DialogResult.Yes
                            TextBox_IncludeFolder.Text = i.SelectedPath
                        Case Else
                            TextBox_IncludeFolder.Text &= ";"c & i.SelectedPath
                    End Select
                End If
            End If
        End Using
    End Sub

    Private Sub Button_AutoAssignPaths_Click(sender As Object, e As EventArgs) Handles Button_AutoAssignPaths.Click
        Using i As New FolderBrowserDialog
            If (String.IsNullOrEmpty(TextBox_AutoAssignPaths.Text)) Then
                i.SelectedPath = IO.Path.GetDirectoryName(TextBox_CompilerPath.Text)
            Else
                i.SelectedPath = TextBox_AutoAssignPaths.Text.Split(";"c)(0)
            End If

            If (i.ShowDialog = DialogResult.OK) Then
                If (String.IsNullOrEmpty(TextBox_AutoAssignPaths.Text)) Then
                    TextBox_AutoAssignPaths.Text = i.SelectedPath
                Else
                    Select Case MessageBox.Show("Replace already existing paths with this one? Otherwise the selected path will be addded to other already existing paths.", "Replace or add paths", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        Case DialogResult.Yes
                            TextBox_AutoAssignPaths.Text = i.SelectedPath
                        Case Else
                            TextBox_AutoAssignPaths.Text &= ";"c & i.SelectedPath
                    End Select
                End If
            End If
        End Using
    End Sub

    Private Sub Button_ClientFolder_Click(sender As Object, e As EventArgs) Handles Button_ClientFolder.Click
        Using i As New FolderBrowserDialog
            If (i.ShowDialog = DialogResult.OK) Then
                Dim sGameConfig As String = IO.Path.Combine(i.SelectedPath, "gameinfo.txt")

                If (Not IO.File.Exists(sGameConfig)) Then
                    MessageBox.Show("Invalid client directory! Game info not found!", "Invalid client directory", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                TextBox_ClientFolder.Text = i.SelectedPath
            End If
        End Using
    End Sub

    Private Sub Button_ServerFolder_Click(sender As Object, e As EventArgs) Handles Button_ServerFolder.Click
        Using i As New FolderBrowserDialog
            If (i.ShowDialog = DialogResult.OK) Then
                Dim sGameConfig As String = IO.Path.Combine(i.SelectedPath, "gameinfo.txt")

                If (Not IO.File.Exists(sGameConfig)) Then
                    MessageBox.Show("Invalid server directory! Game info not found!", "Invalid server directory", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                TextBox_ServerFolder.Text = i.SelectedPath
            End If
        End Using
    End Sub

    Private Sub Button_SourceModFolder_Click(sender As Object, e As EventArgs) Handles Button_SourceModFolder.Click
        Using i As New FolderBrowserDialog
            If (i.ShowDialog = DialogResult.OK) Then
                Dim sSourceModBin As String = IO.Path.Combine(i.SelectedPath, "bin\sourcemod_mm.dll")

                If (Not IO.File.Exists(sSourceModBin)) Then
                    MessageBox.Show("Invalid SourceMod directory! SourceMod not found!", "Invalid SourceMod directory", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                TextBox_SourceModFolder.Text = i.SelectedPath
            End If
        End Using
    End Sub

    Private Sub Button_SaveConfig_Click(sender As Object, e As EventArgs) Handles Button_SaveConfig.Click
        Try
            SaveSelectedConfig()
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub Button_ConfigCopy_Click(sender As Object, e As EventArgs) Handles Button_ConfigCopy.Click
        Try
            PromptSaveSelectedConfig()

            If (g_mListBoxConfigSelectedItem Is Nothing) Then
                Return
            End If

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString

            Dim mConfig As ClassConfigs.STRUC_CONFIG_ITEM = ClassConfigs.LoadConfig(sName)
            If (mConfig Is Nothing) Then
                MessageBox.Show("Current config not found or default config!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            mConfig.SetName(String.Format("{0} {1}", mConfig.GetName, Guid.NewGuid.ToString))

            If (Not mConfig.SaveConfig) Then
                MessageBox.Show("Failed to save copy!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ListBox_Configs.Items.Add(mConfig.GetName)

            Dim i As Integer = ListBox_Configs.FindStringExact(mConfig.GetName)
            If (i > -1) Then
                ListBox_Configs.SetSelected(i, True)
            End If

            MarkChanged()

            g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnConfigChanged())
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub Button_ConfigRename_Click(sender As Object, e As EventArgs) Handles Button_ConfigRename.Click
        Try
            PromptSaveSelectedConfig()

            If (g_mListBoxConfigSelectedItem Is Nothing) Then
                Return
            End If

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString
            Dim sNewName As String = TextBox_ConfigName.Text

            If (String.IsNullOrEmpty(sNewName) OrElse ClassConfigs.m_DefaultConfig.GetName = sNewName OrElse sNewName.IndexOfAny(IO.Path.GetInvalidFileNameChars) > -1 OrElse sNewName.IndexOfAny(IO.Path.GetInvalidPathChars) > -1) Then
                MessageBox.Show("Invalid config name!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim mConfig As ClassConfigs.STRUC_CONFIG_ITEM = ClassConfigs.LoadConfig(sName)
            If (mConfig Is Nothing) Then
                MessageBox.Show("Current config not found or default config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim mNewConfig As ClassConfigs.STRUC_CONFIG_ITEM = ClassConfigs.LoadConfig(sNewName)
            If (mNewConfig IsNot Nothing) Then
                Select Case (MessageBox.Show("This config name is already used! Overwrite config?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    Case DialogResult.No
                        Return
                End Select
            End If

            mConfig.RemoveConfig()
            mConfig.SetName(sNewName)
            mConfig.SaveConfig()

            ListBox_Configs.Items.Remove(sName)
            ListBox_Configs.Items.Remove(sNewName)
            ListBox_Configs.Items.Add(sNewName)

            Dim i As Integer = ListBox_Configs.FindStringExact(sNewName)
            If (i > -1) Then
                ListBox_Configs.SetSelected(i, True)
            End If

            MarkChanged()

            g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnConfigChanged())
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub LinkLabel_ShowShellArguments_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel_ShowShellArguments.LinkClicked
        Try
            Dim SB As New Text.StringBuilder
            SB.AppendLine("All available shell arguments:")
            SB.AppendLine()

            For Each iItem In ClassSettings.GetShellArguments(g_mFormMain, Nothing)
                SB.AppendFormat("{0} - {1}", iItem.g_sMarker, iItem.g_sArgumentName).AppendLine()
            Next

            MessageBox.Show(SB.ToString, "Information", MessageBoxButtons.OK)
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Function PromptSaveSelectedConfig() As Boolean
        If (m_ConfigSettingsChanged) Then
            If (g_mListBoxConfigSelectedItem Is Nothing) Then
                Return False
            End If

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString

            Select Case (MessageBox.Show(String.Format("You have made changes to the selected config. Do you want to save the config '{0}'?", sName), "Config changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                Case DialogResult.Yes
                    SaveSelectedConfig()
                    Return True
            End Select
        End If

        Return False
    End Function

    Private Sub SaveSelectedConfig()
        If (g_mListBoxConfigSelectedItem Is Nothing) Then
            Return
        End If

        Dim sName As String = g_mListBoxConfigSelectedItem.ToString

        Dim mConfig As ClassConfigs.STRUC_CONFIG_ITEM = ClassConfigs.LoadConfig(sName)
        If (mConfig Is Nothing) Then
            MessageBox.Show("Current config not found or default config!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If (CheckBox_ConfigIsDefault.Checked) Then
            For Each mTmpConfig As ClassConfigs.STRUC_CONFIG_ITEM In ClassConfigs.GetConfigs()
                If (mTmpConfig.g_bAutoload) Then
                    mTmpConfig.g_bAutoload = False
                    mTmpConfig.SaveConfig()
                End If
            Next
        End If

        Dim mCompilerOptionsSP As New ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.STRUC_SP_COMPILER_OPTIONS
        Dim mCompilerOptionsAMXX As New ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.STRUC_AMXX_COMPILER_OPTIONS

        'SourcePawn
        If (True) Then
            Select Case (ComboBox_COOptimizationLevelSP.SelectedIndex)
                Case 1
                    mCompilerOptionsSP.g_iOptimizationLevel = 0
                Case 2
                    mCompilerOptionsSP.g_iOptimizationLevel = 2
            End Select

            Select Case (ComboBox_COVerbosityLevelSP.SelectedIndex)
                Case 1
                    mCompilerOptionsSP.g_iVerbosityLevel = 0
                Case 2
                    mCompilerOptionsSP.g_iVerbosityLevel = 1
                Case 3
                    mCompilerOptionsSP.g_iVerbosityLevel = 2
            End Select

            Select Case (ComboBox_COTreatWarningsAsErrorsSP.SelectedIndex)
                Case 1
                    mCompilerOptionsSP.g_iTreatWarningsAsErrors = 1
                Case 2
                    mCompilerOptionsSP.g_iTreatWarningsAsErrors = 0
            End Select

            mCompilerOptionsSP.g_lIgnoredWarnings.Clear()
            ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.ParseIgnoredWarnings(TextBoxEx_COIgnoredWarningsSP.m_NoWatermarkText, mCompilerOptionsSP.g_lIgnoredWarnings)

            mCompilerOptionsSP.g_mDefineConstants.Clear()
            ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.ParseDefineConstants(TextBoxEx_CODefineConstantsSP.m_NoWatermarkText, mCompilerOptionsSP.g_mDefineConstants)
        End If

        'AMX Mod X
        If (True) Then
            Select Case (ComboBox_COSymbolicInformationAMXX.SelectedIndex)
                Case 1
                    mCompilerOptionsAMXX.g_iSymbolicInformation = 0
                Case 2
                    mCompilerOptionsAMXX.g_iSymbolicInformation = 1
                Case 3
                    mCompilerOptionsAMXX.g_iSymbolicInformation = 2
                Case 4
                    mCompilerOptionsAMXX.g_iSymbolicInformation = 3
            End Select

            Select Case (ComboBox_COVerbosityLevelAMXX.SelectedIndex)
                Case 1
                    mCompilerOptionsAMXX.g_iVerbosityLevel = 0
                Case 2
                    mCompilerOptionsAMXX.g_iVerbosityLevel = 1
                Case 3
                    mCompilerOptionsAMXX.g_iVerbosityLevel = 2
            End Select

            Select Case (ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndex)
                Case 1
                    mCompilerOptionsAMXX.g_iTreatWarningsAsErrors = 1
                Case 2
                    mCompilerOptionsAMXX.g_iTreatWarningsAsErrors = 0
            End Select

            mCompilerOptionsAMXX.g_lIgnoredWarnings.Clear()
            ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.ParseIgnoredWarnings(TextBoxEx_COIgnoredWarningsAMXX.m_NoWatermarkText, mCompilerOptionsAMXX.g_lIgnoredWarnings)

            mCompilerOptionsAMXX.g_mDefineConstants.Clear()
            ClassConfigs.STRUC_CONFIG_ITEM.CompilerOptions.ParseDefineConstants(TextBoxEx_CODefineConstantsAMXX.m_NoWatermarkText, mCompilerOptionsAMXX.g_mDefineConstants)
        End If

        ClassConfigs.SaveConfig(New ClassConfigs.STRUC_CONFIG_ITEM(sName,
                                                                    If(RadioButton_ConfigSettingManual.Checked, ClassSettings.ENUM_COMPILING_TYPE.CONFIG, ClassSettings.ENUM_COMPILING_TYPE.AUTOMATIC),
                                                                    TextBox_IncludeFolder.Text,
                                                                    TextBox_CompilerPath.Text,
                                                                    TextBox_OutputFolder.Text,
                                                                    CheckBox_ConfigIsDefault.Checked,
                                                                    TextBox_AutoAssignPaths.Text,
                                                                    CType(ComboBox_Language.SelectedIndex, ClassConfigs.STRUC_CONFIG_ITEM.ENUM_LANGUAGE_DETECT_TYPE),
                                                                    mCompilerOptionsSP,
                                                                    mCompilerOptionsAMXX,
                                                                    TextBox_ClientFolder.Text,
                                                                    TextBox_ServerFolder.Text,
                                                                    TextBox_SourceModFolder.Text,
                                                                    TextBox_Shell.Text))

        m_ConfigSettingsChanged = False

        g_mFormMain.g_ClassPluginController.PluginsExecute(Sub(j As ClassPluginController.STRUC_PLUGIN_ITEM) j.mPluginInterface.OnConfigChanged())
    End Sub

    Private Sub RadioButton_ConfigSettingAutomatic_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_ConfigSettingAutomatic.CheckedChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub RadioButton_ConfigSettingManual_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_ConfigSettingManual.CheckedChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_CompilerPath_TextChanged(sender As Object, e As EventArgs) Handles TextBox_CompilerPath.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_IncludeFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBox_IncludeFolder.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_OutputFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBox_OutputFolder.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_AutoAssignPaths_TextChanged(sender As Object, e As EventArgs) Handles TextBox_AutoAssignPaths.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub CheckBox_ConfigIsDefault_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_ConfigIsDefault.CheckedChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_ClientFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBox_ClientFolder.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_ServerFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBox_ServerFolder.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_SourceModFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBox_SourceModFolder.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBox_Shell_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Shell.TextChanged
        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_Language_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Language.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COOptimizationLevelSP_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COOptimizationLevelSP.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COVerbosityLevelSP_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COVerbosityLevelSP.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COTreatWarningsAsErrorsSP_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COTreatWarningsAsErrorsSP.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBoxEx_COIgnoredWarningsSP_TextChanged(sender As Object, e As EventArgs) Handles TextBoxEx_COIgnoredWarningsSP.TextChanged
        If (TextBoxEx_COIgnoredWarningsSP.m_WatermarkVisible) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBoxEx_CODefineConstantsSP_TextChanged(sender As Object, e As EventArgs) Handles TextBoxEx_CODefineConstantsSP.TextChanged
        If (TextBoxEx_CODefineConstantsSP.m_WatermarkVisible) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COSymbolicInformationAMXX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COSymbolicInformationAMXX.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COVerbosityLevelAMXX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COVerbosityLevelAMXX.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub ComboBox_COTreatWarningsAsErrorsAMXX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_COTreatWarningsAsErrorsAMXX.SelectedIndexChanged
        If (g_bComboBoxIgnoreEvent) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBoxEx_COIgnoredWarningsAMXX_TextChanged(sender As Object, e As EventArgs) Handles TextBoxEx_COIgnoredWarningsAMXX.TextChanged
        If (TextBoxEx_COIgnoredWarningsAMXX.m_WatermarkVisible) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Private Sub TextBoxEx_CODefineConstantsAMXX_TextChanged(sender As Object, e As EventArgs) Handles TextBoxEx_CODefineConstantsAMXX.TextChanged
        If (TextBoxEx_CODefineConstantsAMXX.m_WatermarkVisible) Then
            Return
        End If

        m_ConfigSettingsChanged = True
        MarkChanged()
    End Sub

    Public Sub MarkChanged()
        If (Not g_bIgnoreChange AndAlso Not TabPage_Configs.Text.EndsWith("*"c)) Then
            TabPage_Configs.Text = TabPage_Configs.Text & "*"
            TabControl1.Refresh()
        End If
    End Sub

    Public Sub ResetChanged()
        If (TabPage_Configs.Text.EndsWith("*"c)) Then
            TabPage_Configs.Text = TabPage_Configs.Text.TrimEnd("*"c)
            TabControl1.Refresh()
        End If
    End Sub

    Private Sub Button_KnownFileAdd_Click(sender As Object, e As EventArgs) Handles Button_KnownFileAdd.Click
        Try
            If (g_mListBoxConfigSelectedItem Is Nothing) Then
                Return
            End If

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString
            Dim mConfig As ClassConfigs.STRUC_CONFIG_ITEM = Nothing

            For Each mFindConfig In ClassConfigs.GetConfigs()
                If (mFindConfig.GetName = sName) Then
                    mConfig = mFindConfig
                    Exit For
                End If
            Next

            If (mConfig Is Nothing) Then
                Throw New ArgumentException("Config not found")
            End If

            Using i As New OpenFileDialog()
                i.Multiselect = True

                If (i.ShowDialog = DialogResult.OK) Then
                    For Each sFile As String In i.FileNames
                        ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(sFile) = mConfig
                    Next

                    RefreshKnownFilesListBox()
                End If
            End Using
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub Button_KnownFileRemove_Click(sender As Object, e As EventArgs) Handles Button_KnownFileRemove.Click
        Try
            For Each mListViewItem As ListViewItem In ListView_KnownFiles.SelectedItems
                Dim sFile As String = CStr(mListViewItem.SubItems(0).Text)

                ClassConfigs.ClassKnownConfigs.m_KnownConfigByFile(sFile) = Nothing
            Next

            RefreshKnownFilesListBox()
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Public Sub RefreshKnownFilesListBox()
        Try
            If (g_mListBoxConfigSelectedItem Is Nothing) Then
                Return
            End If

            Dim lListViewItems As New List(Of ListViewItem)

            Dim sName As String = g_mListBoxConfigSelectedItem.ToString

            For Each mKnownConfig In ClassConfigs.ClassKnownConfigs.GetKnownConfigs
                If (mKnownConfig.sConfigName = sName) Then
                    lListViewItems.Add(New ListViewItem(New String() {mKnownConfig.sFile}))
                End If
            Next

            ListView_KnownFiles.BeginUpdate()
            ListView_KnownFiles.Items.Clear()
            ListView_KnownFiles.Items.AddRange(lListViewItems.ToArray)
            ClassTools.ClassControls.ClassListView.AutoResizeColumns(ListView_KnownFiles)
            ListView_KnownFiles.EndUpdate()
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub
#End Region

#Region "Plugins"
    Private Sub ToolStripMenuItem_PluginsRefresh_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_PluginsRefresh.Click
        UpdatePluginsListView()
    End Sub

    Private Sub ContextMenuStrip_Plugins_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip_Plugins.Opening
        ToolStripMenuItem_OpenUrl.Enabled = (ListView_Plugins.SelectedItems.Count > 0)
        ToolStripMenuItem_PluginsEnable.Enabled = (ListView_Plugins.SelectedItems.Count > 0)
        ToolStripMenuItem_PluginsDisable.Enabled = (ListView_Plugins.SelectedItems.Count > 0)
    End Sub

    Private Sub ToolStripMenuItem_OpenUrl_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_OpenUrl.Click
        Try
            For Each mItem As ListViewItem In ListView_Plugins.SelectedItems
                Dim sURL As String = mItem.SubItems(5).Text

                If (String.IsNullOrEmpty(sURL) OrElse Not sURL.StartsWith("http")) Then
                    MessageBox.Show("Unable to open URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Continue For
                End If

                Process.Start(sURL)
            Next
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub ToolStripMenuItem_PluginsEnable_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_PluginsEnable.Click
        Try
            For Each mItem As ListViewItem In ListView_Plugins.SelectedItems
                SetPluginState(mItem.SubItems(0).Text, True)
            Next
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try

        UpdatePluginsListView()
    End Sub

    Private Sub ToolStripMenuItem_PluginsDisable_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_PluginsDisable.Click
        Try
            For Each mItem As ListViewItem In ListView_Plugins.SelectedItems
                SetPluginState(mItem.SubItems(0).Text, False)
            Next
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try

        UpdatePluginsListView()
    End Sub

    Private Sub UpdatePluginsListView()
        Try
            Dim lListViewItems As New List(Of ListViewItem)

            For Each mPlugin In g_mFormMain.g_ClassPluginController.m_Plugins
                Try
                    Dim iPluginState As ENUM_PLUGIN_IMAGE_STATE = If(mPlugin.mPluginInterface.m_PluginEnabled, ENUM_PLUGIN_IMAGE_STATE.ENABLED, ENUM_PLUGIN_IMAGE_STATE.DISABLED)

                    If (mPlugin.mPluginInformation Is Nothing) Then
                        lListViewItems.Add(New ListViewItem(New String() {
                                                            IO.Path.GetFileName(mPlugin.sFile),
                                                            "-",
                                                            "-",
                                                            "-",
                                                            "-",
                                                            "-",
                                                            ""
                                                       }, CStr(iPluginState)))
                    Else
                        lListViewItems.Add(New ListViewItem(New String() {
                                                                    IO.Path.GetFileName(mPlugin.sFile),
                                                                    mPlugin.mPluginInformation.sName,
                                                                    mPlugin.mPluginInformation.sAuthor,
                                                                    mPlugin.mPluginInformation.sDescription,
                                                                    mPlugin.mPluginInformation.sVersion,
                                                                    mPlugin.mPluginInformation.sURL,
                                                                    ""
                                                               }, CStr(iPluginState)))
                    End If
                Catch ex As Exception
                    lListViewItems.Add(New ListViewItem(New String() {
                                                                    IO.Path.GetFileName(mPlugin.sFile),
                                                                    "-",
                                                                    "-",
                                                                    "-",
                                                                    "-",
                                                                    "-",
                                                                    ex.Message
                                                               }, CStr(ENUM_PLUGIN_IMAGE_STATE.ENABLED)))

                    ClassExceptionLog.WriteToLogMessageBox(ex)
                End Try
            Next

            For Each mPlugin In g_mFormMain.g_ClassPluginController.m_FailPlugins
                lListViewItems.Add(New ListViewItem(New String() {
                                                    IO.Path.GetFileName(mPlugin.sFile),
                                                    "-",
                                                    "-",
                                                    "-",
                                                    "-",
                                                    "-",
                                                    mPlugin.mException.Message
                                                }, CStr(ENUM_PLUGIN_IMAGE_STATE.DISABLED)))
            Next

            ListView_Plugins.BeginUpdate()
            ListView_Plugins.Items.Clear()
            ListView_Plugins.Items.AddRange(lListViewItems.ToArray)
            ClassTools.ClassControls.ClassListView.AutoResizeColumns(ListView_Plugins)
            ListView_Plugins.EndUpdate()
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub SetPluginState(sFilename As String, bEnable As Boolean)
        Try
            For Each mPlugin In g_mFormMain.g_ClassPluginController.m_Plugins
                If (IO.Path.GetFileName(mPlugin.sFile).ToLower <> sFilename.ToLower) Then
                    Continue For
                End If

                While True
                    Dim bSuccess As Boolean = False
                    Dim sReason As String = ""

                    If (bEnable) Then
                        bSuccess = mPlugin.mPluginInterface.OnPluginEnabled(sReason)
                    Else
                        bSuccess = mPlugin.mPluginInterface.OnPluginDisabled(sReason)
                    End If

                    If (bSuccess) Then
                        g_mFormMain.g_ClassPluginController.m_PluginEnabledByConfig(mPlugin) = bEnable
                    Else
                        If (String.IsNullOrEmpty(sReason)) Then
                            sReason = "Unknown"
                        End If

                        With New Text.StringBuilder
                            .AppendFormat("Could not change plugin state of plugin '{0}' with reason:", mPlugin.sFile).AppendLine()
                            .AppendLine()
                            .AppendLine(sReason)

                            Select Case (MessageBox.Show(.ToString, "Chould not change plugin state", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                                Case DialogResult.Retry
                                    Continue While
                            End Select
                        End With
                    End If

                    Exit While
                End While
            Next
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub

    Private Sub LinkLabel_MorePlugins_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel_MorePlugins.LinkClicked
        Try
            Process.Start("https://github.com/Timocop/BasicPawn/tree/master/Plugin%20Releases")
        Catch ex As Exception
            ClassExceptionLog.WriteToLogMessageBox(ex)
        End Try
    End Sub
#End Region

#Region "Database"
    Private Sub Button_AddDatabaseItem_Click(sender As Object, e As EventArgs) Handles Button_AddDatabaseItem.Click
        Using i As New FormDatabaseInput()
            If (i.ShowDialog(Me) = DialogResult.OK) Then
                DatabaseListBox_Database.BeginUpdate()
                DatabaseListBox_Database.RemoveItemByName(i.m_Name)
                DatabaseListBox_Database.Items.Add(New ClassDatabaseListBox.ClassDatabaseItem(i.m_Name, i.m_Username))
                DatabaseListBox_Database.EndUpdate()

                Dim iItem As New ClassDatabase.STRUC_DATABASE_ITEM(i.m_Name, i.m_Username, i.m_Password)
                iItem.Save()
            End If
        End Using
    End Sub

    Private Sub Button_Refresh_Click(sender As Object, e As EventArgs) Handles Button_Refresh.Click
        DatabaseListBox_Database.FillFromDatabase()
    End Sub
#End Region

End Class