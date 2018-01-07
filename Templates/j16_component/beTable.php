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

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

/**
 * <#= Value("Extension.name") #> Table
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Table<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JTable{

	/**
	 * Constructor
	 *
	 * @param JDatabase A database connector object
	 */
	public function __construct(&$db)
	{
		parent::__construct('#__<#= Value("Task.table") #>', '<#= Value("Task.primaryKey") #>', $db);
	}
	
	function check(){
		// write here data validation code
		return parent::check();
	}
}