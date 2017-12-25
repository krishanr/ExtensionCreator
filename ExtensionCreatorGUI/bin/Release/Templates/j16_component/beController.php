<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * <#= Value("Extension.name") #> Controller for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */
// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

jimport('joomla.application.component.controllerform');

class <#= Value("Extension.name") #>Controller<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JControllerForm {
	/**
	 * @var		string	The prefix to use with controller messages.
	 * @since	1.6
	 */
	protected $text_prefix = 'COM_<#= StrConv(Value("Extension.name") & "_" & Value("Task.nameObject"), VbStrConv.UpperCase) #>';
	<# 'Set the controller messages. #>
	<# JExt.GenText(Value("Task.nameObject") & " save success", StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " successfully saved.") #>
	
	/**
	 * @var    string	The URL view list variable.
	 * @since  11.1
	 */
	protected $view_list = '<#= Value("Task.nameObjectList") #>';
}