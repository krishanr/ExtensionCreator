<#@ template inherits="ETemplate" language="VB" #>
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

/**
 * <#= Value("Extension.name") #> Model
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JModelLegacy {
	
	
}
