<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * <#= Value("Extension.name") #> Helper for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// No direct access
defined('_JEXEC') or die;

/**
 * <#= Value("Extension.label") #> Helper.
 */
class <#= Value("Extension.name") #>Helper
{
	/**
	 * Configure the Linkbar.
	 */
	public static function addSubmenu($vName = '')
	{
	<# Dim Task #>
	<# For Each Task In JExt.GetTasks("*", "menu", "link", "view", "image", "alt", "label", "labelKey","nameObjectList") #>
	<# Dim MenuTitle, MenuLink, MenuView as string #>
	<# If strconv(Task.menu , VbStrConv.Lowercase) = "true" Then #>
		<# If Task.label <> "" #>
			<# MenuTitle = JExt.GenText(Task.labelKey, Task.label, JExt.LangFiles.Item("admin"))  #>
		<# Else #>
			<# MenuTitle = JExt.GenText("submenu " & Task.nameObjectList, StrConv(Task.nameObjectList, VbStrConv.Propercase)) #>
		<# End If #>
		<# If Task.link <> "" #>
			<# MenuLink = "index.php?" & Task.link #>
		<# Else #>
			<# MenuLink = "index.php?option=com_" & StrConv(Value("Extension.name"), VbStrConv.Lowercase) & "&view=" & Task.nameObjectList  #>
		<# End If #>
		<# If Task.view <> "" #>
			<# MenuView = Task.view #>
		<# Else #>
			<# MenuView = Task.nameObjectList  #>
		<# End If #>
		JSubMenuHelper::addEntry(
			JText::_('<#= MenuTitle #>'),
			'<#= MenuLink #>',
			$vName == '<#= MenuView #>'
		);	
	<# End if#>
	<# Next #>
	}
}
