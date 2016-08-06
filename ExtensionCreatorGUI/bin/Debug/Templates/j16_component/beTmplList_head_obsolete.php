<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# Dim row #>
<?php $this->NumCols = 0; ?>
<?php
/**
 * default head template file for <#= Value("Task.nameObjectList") #> view of <#= Value("Extension.name") #> component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */
// No direct access to this file
defined('_JEXEC') or die('Restricted Access');
?>
<tr>
	<?php $this->NumCols++; ?>
	<th width="1%">
		<input type="checkbox" name="toggle" value="" title="<?php echo JText::_('JGLOBAL_CHECK_ALL'); ?>" onclick="checkAll(this)" />
	</th>
<# For Each row In JComponentDataTable.Rows #>
<#     If row.ShowInList = True Then #>
	<?php $this->NumCols++; ?>
	<th>
		<?php echo JHTML::_('grid.sort', '<#= row.ListLabel #>', '<#= Value("Task.tableAlias") #>.<#= row.Field #>', $this->listDirn, $this->listOrder); ?>
	</th>
<#     End If #>
<# Next #>
</tr>

