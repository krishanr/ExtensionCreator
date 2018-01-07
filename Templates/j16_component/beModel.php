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

// import Joomla modelform library
jimport('joomla.application.component.modeladmin');

/**
 * <#= Value("Extension.name") #> Model
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JModelAdmin {	
	/**
	 * Method to get the record form.
	 *
	 * @param	array	$data		Data for the form.
	 * @param	boolean	$loadData	True if the form is to load its own data (default case), false if not.
	 * @return	mixed	A JForm object on success, false on failure
	 * @since	1.6
	 */
	public function getForm($data = array(), $loadData = true) 
	{
		// Get the form.
		$form = $this->loadForm("$this->option.".$this->getName(), $this->getName(), array('control' => 'jform', 'load_data' => $loadData));
		if (empty($form)) 
		{
			return false;
		}
		return $form;
	}
	
	/**
	 * Returns a reference to a Table object, always creating it.
	 *
	 * @param	type	The table type to instantiate
	 * @param	string	A prefix for the table class name. Optional.
	 * @param	array	Configuration array for model. Optional.
	 * @return	JTable	A database object
	 * @since	1.6
	 */
	public function getTable($type = '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>', $prefix = '<#= Value("Extension.name") #>Table', $config = array()) 
	{
		return parent::getTable($type, $prefix, $config);
	}
	
	/**
	 * Method to get the data that should be injected in the form.
	 *
	 * @return	mixed	The data for the form.
	 * @since	1.6
	 */
	protected function loadFormData() 
	{
		// Check the session for previously entered form data.
		$data = JFactory::getApplication()->getUserState("$this->option.edit.".$this->getName().".data", array());
		if (empty($data)) 
		{
			$data = $this->getItem();
		}
		return $data;
	}
}