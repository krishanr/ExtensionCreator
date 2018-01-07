<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Extension.name") #> Model for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// Check to ensure this file is included in Joomla!
defined('_JEXEC') or die();

jimport( 'joomla.application.component.model' );

/**
 * <#= Value("Extension.name") #> Model
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JModel{

}
