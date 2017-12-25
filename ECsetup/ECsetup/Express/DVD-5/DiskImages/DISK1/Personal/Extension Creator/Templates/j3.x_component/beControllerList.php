<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * <#= Value("Extension.name") #> Model for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 *
 */

// No direct access
defined('_JEXEC') or die( 'Restricted access' );

class <#= Value("Extension.name") #>Controller<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JControllerAdmin {
	/**
	 * @var		string	The prefix to use with controller messages.
	 * @since	1.6
	 */
	protected $text_prefix = 'COM_<#= StrConv(Value("Extension.name") & "_" & Value("Task.nameObjectList"), VbStrConv.UpperCase) #>';
<#     'Set the controller messages.
	   JExt.GenText(Value("Task.nameObjectList") & " n items trashed", "%s " & StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) & " trashed.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items trashed 1", "%s " & StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " trashed.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items published", "%s " & StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) & " published.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items published 1", "%s " & StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " published.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items unpublished", "%s " & StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) & " unpublished.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items unpublished 1", "%s " & StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " unpublished.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items deleted", "%s " & StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) & " deleted.")
	   JExt.GenText(Value("Task.nameObjectList") & " n items deleted 1", "%s " & StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " deleted.") 
	   JExt.GenText(Value("Task.nameObjectList") & " no item selected", "No " & StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) & " selected.") #>

	/**
	 * Proxy for getModel
	 *
	 * @param   string  $name    The model name. Optional.
	 * @param   string  $prefix  The class prefix. Optional.
	 * @param   array   $config  An optional associative array of configuration settings.
	 *
	 * @return  JModelLegacy  The model.
	 *
	 * @since   3.1
	 */
	public function getModel($name = '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>', $prefix = '<#= Value("Extension.name") #>Model', $config = array('ignore_request' => true))
	{
		$model = parent::getModel($name, $prefix, $config);
		return $model;
	}
}