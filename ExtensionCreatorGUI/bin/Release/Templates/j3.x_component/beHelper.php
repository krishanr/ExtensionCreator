<#@ template inherits="ETemplate" language="VB" #>
<#@ import namespace="System.Linq" #>
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
<#	   Dim Tasks = From Task In JExt.TaskDictionary
					   Where Task.Value.item("TaskName") Like "*"
					   Select Task.Value.item("TaskParameters")
	   Dim Menu
	   For Each Menu In Tasks
	   Dim MenuTitle, MenuLink, MenuView as string
	   If Menu.ContainsKey("menu") AndAlso  Boolean.Parse(Menu("menu")) Then 
			MenuTitle = If(Menu.ContainsKey("label"), JExt.GenText(Menu("labelKey"), Menu("label"), JExt.LangFiles.Item("admin")), _
		   JExt.GenText("submenu " & Menu("nameObjectList"), StrConv(Menu("nameObjectList"), VbStrConv.Propercase)))
			MenuLink = If(Menu.ContainsKey("link"), "index.php?" & Menu("link"), _
		   "index.php?option=com_" & StrConv(Value("Extension.name"), VbStrConv.Lowercase) & "&view=" & Menu("nameObjectList"))
			MenuView = If(Menu.ContainsKey("view"), Menu("view"), _
			Menu("nameObjectList"))
		#>
		JHtmlSidebar::addEntry(
			JText::_('<#= MenuTitle #>'),
			'<#= MenuLink #>',
			$vName == '<#= MenuView #>'
		);	
<#	   End if
	   Next #>
	}
}
