<#@ template inherits="ETemplate" language="VB" #>
<?php
// No direct access.
defined('_JEXEC') or die;

JHtml::_('behavior.formvalidator');
JHtml::_('behavior.keepalive');
JHtml::_('formbehavior.chosen', 'select');

JFactory::getDocument()->addScriptDeclaration('
	Joomla.submitbutton = function(task)
	{
		if (task == "<#= Value("Task.nameObject") #>.cancel" || document.formvalidator.isValid(document.getElementById("item-form")))
		{
			Joomla.submitform(task, document.getElementById("item-form"));
		}
	};
');
?>

<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&layout=edit&<#= Value("Task.primaryKey") #>='. $this->item-><#= Value("Task.primaryKey") #>); ?>" method="post" name="adminForm" id="item-form" class="form-validate">

	<div class="form-horizontal">
		<?php echo JHtml::_('bootstrap.startTabSet', 'myTab', array('active' => 'main')); ?>

		<?php echo JHtml::_('bootstrap.addTab', 'myTab', 'main', '<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> Information'); ?>
		<div class="row-fluid">
			<div class="span9">
				<?php foreach ($this->form->getFieldsets() as $fieldsets => $fieldset) : ?>
				<div>
					<?php echo $this->form->renderFieldset($fieldset->name); ?>
				</div>
				<?php endforeach; ?>
			</div>
		</div>
		<?php echo JHtml::_('bootstrap.endTab'); ?>

		<?php echo JHtml::_('bootstrap.endTabSet'); ?>
	</div>
	
	<input type="hidden" name="task" value="" />
	<?php echo JHtml::_('form.token'); ?>
</form>