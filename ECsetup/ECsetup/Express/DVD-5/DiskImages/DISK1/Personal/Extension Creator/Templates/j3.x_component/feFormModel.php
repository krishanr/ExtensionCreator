<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
/**
 * <#= Value("Extension.name") #> Model for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// No direct access
defined('_JEXEC') or die( 'Restricted access' );
 
/**
 * <#= Value("Extension.name") #> Model
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JModelAdmin
{
	protected $_formName = null;
	
	/**
	 * Constructor.
	 *
	 * @param   array  $config	An optional associative array of configuration settings.
	 *
	 * @see		JController
	 * @since  11.1
	 */
	public function __construct($config = array())
	{
		parent::__construct($config);
		
		if (isset($config['form_name'])) {
			$this->_formName = $config['form_name'];
		} else  if (empty($this->_formName)) {
			// Guess the form name as the suffix, eg: OptionModelAppointment.
			$r = null;
			if (!preg_match('/(.*)Model(.*)/i', get_class($this), $r)) {
				JError::raiseError(500, "JModel: ".ucfirst($this->getName())." : Cannot get or parse form name.");
			}
			$this->_formName = strtolower($r[2]);
		}
		if (!empty($config['ignore_request'])) {
			//Set the JForm paths in this case, since this model could
			//be getting called from a module (for example).
			$comPath = JPATH_BASE . '/components/' . $this->option;
			JForm::addFormPath($comPath.'/models/forms');
			JForm::addFieldPath($comPath.'/models/fields');
		}
	}
	
	public function getFormName() {
		return $this->_formName;
	}
	
	/**
	 * Returns a reference to the a Table object, always creating it.
	 *
	 * @param	type	The table type to instantiate
	 * @param	string	A prefix for the table class name. Optional.
	 * @param	array	Configuration array for model. Optional.
	 * @return	JTable	A database object
	 * @since	1.6
	 */
	public function getTable($type = '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>', $prefix = '<#= Value("Extension.name") #>Table', $config = array()) 
	{
		return JTable::getInstance($type, $prefix, $config);
	}

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
		//First param of loadForm uniquely identifies the form in the cache
		$form = $this->loadForm("$this->option.".$this->getFormName(), $this->getFormName(), array('control' => 'jform', 'load_data' => $loadData));
		if (empty($form)) 
		{
			return false;
		}
		return $form;
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
		$context = "$this->option.edit.".$this->getFormName();
		$data = JFactory::getApplication()->getUserState($context.'.data', array());
		if (empty($data)) 
		{
			$data = null;
		}
		return $data;
	}
	
	/**
	 * Method to auto-populate the model state.
	 *
	 * Note. Calling getState in this method will result in recursion.
	 *
	 * @since	1.6
	 */
	protected function populateState()
	{
		$app = JFactory::getApplication('site');
		$context	= "$this->option.edit.".$this->getFormName();
		
		//TODO: Not a secure way to get id
		// Load state from user state
		$pk = (int) $app->getUserState($context.'.id', 0);
		//Remove the user state
		$app->setUserState($context.'.id', 0);
		$this->setState($this->getName().'.id', $pk);
		
		// Load the parameters.
		$params = $app->getParams();
		$this->setState('params', $params);
	}
	
	/**
	 * Method to save the form data.
	 *
	 * @param	array	The form data.
	 *
	 * @return	boolean	True on success.
	 * @since	1.6
	 */
	public function save($data)
	{
		if (parent::save($data)) {
			return true;
		} else {
			return false;			
		}
	}
	
	/**
	 * Method to validate the form data.
	 *
	 * @param   object  $form		The form to validate against.
	 * @param   array   $data		The data to validate.
	 * @return  mixed  Array of filtered data if valid, false otherwise.
	 * @since	1.1
	 */
	function validate($form, $data)
	{
		return parent::validate($form, $data);
	}
	
	/**
	 * Method to get a single record.
	 *
	 * @param   integer  $pk	The id of the primary key.
	 *
	 * @return  mixed    Object on success, false on failure.
	 * @since   11.1
	 */
	public function getItem($pk = null)
	{
		return parent::getItem($pk);
	}
}