<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# Dim row #>
<?php
/**
 * <#= Value("Extension.name") #> Model for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

// import the Joomla modellist library
jimport('joomla.application.component.modellist');
jimport('joomla.form.form');

/**
 * <#= Value("Extension.name") #> Model List
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JModelList {
	/**
	 * Array of form objects.
	 */
	protected $_forms = array();
		
	/**
	 * Constructor.
	 *
	 * @param   array  An optional associative array of configuration settings.
	 * @see		JController
	 */
	public function __construct($config = array())
	{
		parent::__construct($config);

		// Add the ordering filtering fields to white list.
		if (empty($this->filter_fields)) {
            <# Dim OrderableFields As New List(Of String) #>
            <# For Each row In JComponentDataTable.Rows #>
			<# 	 OrderableFields.Add(Value("Task.tableAlias") & "." & row.Field) #>
           <# Next #>
			$this->filter_fields = explode(',','<#= Join(OrderableFields.ToArray(), ",") #>');
		}
	}

	/**
	 * Method to get the record form.
	 *
	 * @param	array	$data		Data for the form.
	 * @param	boolean	$loadData	True if the form is to load its own data (default case), false if not.
	 * @return	mixed	A JForm object on success, false on failure
	 */
	public function getForm($data = array(), $loadData = true, $clear = false) 
	{
		$options = array('control' => '', 'load_data' => $loadData);
		// Handle the optional arguments.
		$options['control']	= JArrayHelper::getValue($options, 'control', false);

		// Create a signature hash.
		$hash = md5($source.serialize($options));

		// Check if we can use a previously loaded form.
		if (isset($this->_forms[$hash]) && !$clear) {
			return $this->_forms[$hash];
		}
		
		// Get the form.
		JForm::addFormPath(JPATH_ADMINISTRATOR.'/components/'.$this->option.'/models/forms');
		JForm::addFieldPath(JPATH_ADMINISTRATOR.'/components/'.$this->option.'/models/fields');
		
		try {
			$form = JForm::getInstance("$this->option.".$this->getName(), $this->getName()."table", $options);

			if (isset($options['load_data']) && $options['load_data']) {
				// Get the data for the form.
				$data = $this->loadFormData();
			} else {
				$data = array();
			}

			// Load the data into the form.
			$form->bind($data);

		} catch (Exception $e) {
			$this->setError($e->getMessage());
			return false;
		}
		// Store the form for later.
		$this->_forms[$hash] = $form;

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
		$properties = array();
		$form = $this->getForm(array(), false);
		foreach ($form->getFieldset('select_lists') as $field) {
			$properties[$field->name] = $this->getState(str_replace('_', '.', $field->name));
		}
		$search_filter = $form->getField('search_filter');
		if($search_filter !== FALSE) {
			$properties['search_filter'] = $this->getState('search.filter');
		}
		return JArrayHelper::toObject($properties, 'JObject');
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
	 * Method to auto-populate the model state.
	 *
	 * Note. Calling getState in this method will result in recursion.
	 *
	 * @return	void
	 * @since	1.6
	 */
	protected function populateState($ordering = null, $direction = null)
	{
		// Initialise variables.
		$app = JFactory::getApplication();
		$session = JFactory::getSession();

		$search = $this->getUserStateFromRequest($this->context.'.filter.search', 'filter_search');
		$this->setState('filter.search', $search);

		$form = $this->getForm(array(), false);
		foreach ($form->getFieldset('select_lists') as $field) {
			$field_default = $form->getFieldAttribute($field->name, 'default', null);
			$field_item = $app->getUserStateFromRequest($this->context . '.' . str_replace('_', '.', $field->name), $field->name, $field_default);
			$this->setState( str_replace('_', '.', $field->name), $field_item);	
		}
		
		$search_filter = $form->getField('search_filter');
		if($search_filter !== FALSE) {
			$field_default = $form->getFieldAttribute('search_filter', 'default', null);
			$field_item = $app->getUserStateFromRequest($this->context . '.search_filter', 'search_filter', $field_default);
			$this->setState( 'search.filter', $field_item);	
		}		
		// List state information.
		parent::populateState('<#= Value("Task.tableAlias") #>.<#= Value("Task.primaryKey") #>', 'asc');
	}
	
	/**
	 * Method to get a store id based on model configuration state.
	 *
	 * This is necessary because the model is used by the component and
	 * different modules that might need different sets of data or different
	 * ordering requirements.
	 *
	 * @param	string		$id	A prefix for the store id.
	 *
	 * @return	string		A store id.
	 * @since	1.6
	 */
	protected function getStoreId($id = '')
	{
		// Compile the store id.
		$id	.= ':'.$this->getState('filter.search');

		return parent::getStoreId($id);
	}
	
	/**
	 * Method to build an SQL query to load the list data.
	 *
	 * @return	string	An SQL query
	 */
	protected function getListQuery() 
	{
		// Create a new query object.
		$db = JFactory::getDBO();
		$query = $db->getQuery(true);
		
		$query->select('<#= Value("Task.tableAlias") #>.*');
		$query->from('#__<#= Value("Task.table") #> AS <#= Value("Task.tableAlias") #>');
		
		// Filter by search in fields
		$search = $this->getState('filter.search');
		$search_filter = $this->getState('search.filter');
		if (!empty($search) && !empty($search_filter)) {
			$where = '((0=1) ';
			$search = $db->Quote('%'.$db->getEscaped($search, true).'%');
			$allowedSearch = explode(',', $search_filter);
			foreach($allowedSearch as $field) {
				if (!$field) continue;
				$where .= " OR ( $field LIKE $search ) ";
			}
			$where .= ')';
			$query->where($where);
		}
			
		$form = $this->getForm(array(), false);
		foreach ($form->getFieldset('select_lists') as $field) {
			$select = $form->getFieldAttribute($field->name, 'select', null);
			if(!is_null($select)) {
				$query->select($select);				
			}
			$join = $form->getFieldAttribute($field->name, 'join', null);
			if(!is_null($join)) {
				$query->join('LEFT',$join);				
			}
			$field_value = $this->getState(str_replace('_', '.', $field->name), NULL);
			//Check that the field was set in the state variables and user has selected
			//a filter.
			if(!is_null($field_value) && $field_value !== '') {
				$field_where = $form->getFieldAttribute($field->name, 'where', null);
				if(!is_null($field_where)) {
					$query->where(str_replace('{0}', $db->quote($field_value), $field_where));					
				} else {
					$field_name = $form->getFieldAttribute($field->name, 'key_field', null);
					//Key field should always exist.
					$query->where("<#= Value("Task.tableAlias") #>.$field_name = " . $db->quote($field_value));						
				}			
			}
		}
				
		// Add the list ordering clause.
		$orderCol	= $this->state->get('list.ordering');
		$orderDirn	= $this->state->get('list.direction');
		$query->order($db->getEscaped($orderCol.' '.$orderDirn));
		
		return $query;
	}
}