<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# 'Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# 'Dim row #>
<?php
/**
 * default template file for <#= Value("Task.nameObjectList") #> view of <#= Value("Extension.name") #> component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */
// No direct access to this file
defined('_JEXEC') or die('Restricted Access');

JHtml::_('behavior.tooltip');
JHtml::_('script','system/multiselect.js',false,true);

$this->user	        = JFactory::getUser();
$this->listOrder	= $this->escape($this->state->get('list.ordering'));
$this->listDirn	    = $this->escape($this->state->get('list.direction'));
?>
<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&view=<#= Value("Task.nameObjectList") #>');?>" method="post" name="adminForm" id="adminForm">
	<fieldset id="filter-bar">
	<?php $search_filter = $this->form->getField('search_filter'); ?>
	<?php if($search_filter !== FALSE) : ?>	
		<div class="filter-search fltlft">
			<label class="filter-search-lbl" for="filter_search"><?php echo JText::_('JSEARCH_FILTER_LABEL'); ?></label>
			<input type="text" name="filter_search" id="filter_search" value="<?php echo $this->escape($this->state->get('filter.search')); ?>" title="<?php echo JText::_('JSEARCH_FILTER'); ?>" />
			<?php echo $search_filter->input; ?>
			<button type="submit" class="btn"><?php echo JText::_('JSEARCH_FILTER_SUBMIT'); ?></button>
			<button type="button" onclick="document.id('filter_search').value='';this.form.submit();"><?php echo JText::_('JSEARCH_FILTER_CLEAR'); ?></button>
		</div>
	<?php endif; ?>
		<div class="filter-select fltrt">
			<?php foreach ($this->form->getFieldset('select_lists') as $field) : ?>
				<?php echo $field->input ?>
			<?php endforeach; ?>
		</div>
	</fieldset>
	<div class="clr"> </div>
	
	<table class="adminlist">
		<thead><?php echo $this->loadTemplate('head');?></thead>
		<tfoot><?php echo $this->loadTemplate('foot');?></tfoot>
		<tbody><?php echo $this->loadTemplate('body');?></tbody>
	</table>
	<div>
		<input type="hidden" name="task" value="" />
		<input type="hidden" name="boxchecked" value="0" />
		<input type="hidden" name="filter_order" value="<?php echo $this->listOrder; ?>" />
		<input type="hidden" name="filter_order_Dir" value="<?php echo $this->listDirn; ?>" />
		<?php echo JHtml::_('form.token'); ?>
	</div>
</form>
