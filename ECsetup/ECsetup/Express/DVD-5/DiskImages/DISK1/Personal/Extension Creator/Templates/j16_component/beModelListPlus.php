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

// import mylibrary modellistplus
jimport('mylibrary.application.component.modellistplus');

/**
 * <#= Value("Extension.name") #> Model List Plus
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Model<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JModelListplus {
		
	/**
	 * Constructor.
	 *
	 * @param   array  An optional associative array of configuration settings.
	 * @see		JController
	 */
	public function __construct($config = array())
	{
		// Add the ordering filtering fields to white list
		if (empty($config['filter_fields'])) {
            <# Dim OrderableFields As New List(Of String) #>
            <# For Each row In JComponentDataTable.Rows #>
			<# 	 OrderableFields.Add(row.Field) #>
           <# Next #>
			$config['filter_fields'] = explode(',','<#= Join(OrderableFields.ToArray(), ",") #>');
		}
		$config['_tableType']   = '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>';
		$config['_tablePrefix'] = '<#= Value("Extension.name") #>Table';
		parent::__construct($config);
	}

	/**
	 * Method to get the record form.
	 *
	 * @param	array	$data		Data for the form.
	 * @param	boolean	$loadData	True if the form is to load its own data (default case), false if not.
	 *
	 * @return	mixed	A JForm object on success, false on failure
	 * @since	1.6
	 */
	public function getForm($data = array(), $loadData = true)
	{
		// Get the form.
		$form = parent::getForm($data, $loadData);
		if (empty($form)) 
		{
			return false;
		}
		return $form;
	}
	
	/**
	 * Method to build an SQL query to load the list data.
	 *
	 * @return	string	An SQL query
	 */
	protected function getListQuery()
	{
		return parent::getListQuery();
	}
}