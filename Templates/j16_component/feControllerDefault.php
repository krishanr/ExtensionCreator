<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> default controller
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

jimport('joomla.application.component.controller');

/**
 * <#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> Component Controller
 *
 * @package	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Controller extends JController
{
	/**
	 * Method to display the view
	 *
	 * @access	public
	 */
	function display()
	{
		parent::display();
	}

}
?>
