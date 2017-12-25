<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Extension.name") #> Controller for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */

// No direct access
defined('_JEXEC') or die( 'Restricted access' );

/**
 * <#= Value("Extension.name") #> default controller
 *
 * @since  3.1
 */
class <#= Value("Extension.name") #>Controller extends JControllerLegacy
{
	/**
	 * @var		string	The default view.
	 * @since	1.6
	 */
	protected $default_view = '<#= Value("Extension.defaultView") #>';
	
	/**
	 * Method to display a view.
	 *
	 * @param   boolean  $cachable   If true, the view output will be cached
	 * @param   array    $urlparams  An array of safe url parameters and their variable types, for valid values see {@link JFilterInput::clean()}.
	 *
	 * @return  JControllerLegacy  This object to support chaining.
	 *
	 * @since   3.1
	 */
	public function display($cachable = false, $urlparams = false)
	{
		return parent::display($cachable);
	}
}