<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# Dim hasPublished = JExt.GetActiveTaskCollection().Item("TaskParameters").ContainsKey("hasPublishedField") #>
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

/**
 * <#= Value("Extension.name") #> Model List
 *
 * @since  3.1
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JModelList {
		
	/**
	 * Constructor.
	 *
	 * @param   array  $config  An optional associative array of configuration settings.
	 *
	 * @see    JController
	 * @since  3.0.3
	 */
	public function __construct($config = array())
	{
		// Add the ordering filtering fields to white list
		if (empty($config['filter_fields'])) {
			$config['filter_fields'] = array(
<#				   For i as integer = 0 To (JComponentDataTable.Rows.Count - 1)
			 	   row = JComponentDataTable.Rows.Item(i)
				   If i < (JComponentDataTable.Rows.Count - 1) Then #>
					'<#= row.Field #>', 'a.<#= row.Field #>',
<#				   Else #>
					'<#= row.Field #>', 'a.<#= row.Field #>'
<#				   End If
				   Next #>
			);
		}

		parent::__construct($config);
	}
	
	/**
	 * Method to auto-populate the model state.
	 *
	 * Note. Calling getState in this method will result in recursion.
	 *
	 * @param   string  $ordering   An optional ordering field.
	 * @param   string  $direction  An optional direction (asc|desc).
	 *
	 * @return  void
	 *
	 * @since   1.6
	 */
	protected function populateState($ordering = null, $direction = null)
	{
		$app = JFactory::getApplication();
	
		$search = $this->getUserStateFromRequest($this->context . '.filter.search', 'filter_search');
		$this->setState('filter.search', $search);
		
<#		If hasPublished Then #>
		$published = $this->getUserStateFromRequest($this->context . '.filter.published', 'filter_published', '');
		$this->setState('filter.published', $published);
<#		End If  #>
	
		// List state information.
		parent::populateState('a.<#= Value("Task.primaryKey")#>', 'desc');
	}
	
	/**
	 * Method to create a query for a list of items.
	 *
	 * @return  string
	 *
	 * @since  3.1
	 */
	protected function getListQuery()
	{
		// Create a new query object.
		$db		= $this->getDbo();
		$query	= $db->getQuery(true);
		
		$query->select('a.*');
		$query->from('#__<#= Value("Task.table") #> AS a');		
		
<#		If hasPublished Then #>
		// Filter by published state
		$published = $this->getState('filter.published');

		if (is_numeric($published))
		{
			$query->where('a.published = ' . (int) $published);
		}
		elseif ($published === '')
		{
			$query->where('(a.published = 0 OR a.published = 1)');
		}
<#		End If  #>

		// Add the list ordering clause.
		$orderCol = $this->state->get('list.ordering', 'a.<#= Value("Task.primaryKey")#>');
		$orderDirn = $this->state->get('list.direction', 'desc');
		
		$query->order($db->escape($orderCol . ' ' . $orderDirn));
		
		return $query;
	}
	
	/**
	 * Method to get a table object, load it if necessary.
	 *
	 * @param   string  $type    The table name. Optional.
	 * @param   string  $prefix  The class prefix. Optional.
	 * @param   array   $config  Configuration array for model. Optional.
	 *
	 * @return  JTable  A JTable object
	 *
	 * @since   3.1
	 */
	public function getTable($type = '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>', $prefix = '<#= Value("Extension.name") #>Table', $config = array())
	{
		return JTable::getInstance($type, $prefix, $config);
	}
}