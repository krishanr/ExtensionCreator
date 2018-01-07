<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# Dim row #>
<?php
/**
 * default body template file for <#= Value("Task.nameObjectList") #> view of <#= Value("Extension.name") #> component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */
// No direct access to this file
defined('_JEXEC') or die('Restricted Access');
?>
<?php foreach ($this->items as $i => $row) :
	<# If StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
	$link = JRoute::_( 'index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&task=<#= Value("Task.nameObject") #>.edit&id='.$row-><#= Value("Task.primaryKey") #>);
	$canEdit	= $this->user->authorise('core.edit',			'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
	$canChange	= $this->user->authorise('core.edit.state',	'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
	<# End If #>
	<# If StrConv(Value("Task.canCreate"), VbStrConv.Lowercase) = "true" Then #>
	$canCreate	= $this->user->authorise('core.create',		'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
	<# End If #>
	?>
	<tr class="row<?php echo $i % 2; ?>">
		<td>
			<?php echo JHTML::_('grid.id', $i, $row-><#= Value("Task.primaryKey") #>); ?>
		</td> 
		<# For Each row In JComponentDataTable.Rows #>
		<#     If row.ShowInList = True Then #>
		<td class="center">
		<#     If row.Linkable = True And StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
		<?php if ($canEdit) : ?>
			<a href="<?php echo $link;?>">
			<?php echo $this->escape($row-><#= row.Field #>); ?>
			</a>
		<?php else : ?>
			<?php echo $this->escape($row-><#= row.Field #>); ?>
		<?php endif; ?>
		<#     Else #>
			<?php echo $this->escape($row-><#= row.Field #>); ?>
		<#     End If #>
		</td>
		<#     End If #>
		<# Next #>
	</tr>
<?php endforeach; ?>
