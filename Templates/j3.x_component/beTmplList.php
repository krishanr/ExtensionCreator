<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<?php
// No direct access.
defined('_JEXEC') or die;

JHtml::_('bootstrap.tooltip');
JHtml::_('behavior.multiselect');
JHtml::_('formbehavior.chosen', 'select');

$user		= JFactory::getUser();
$userId		= $user->get('id');
$listOrder	= $this->escape($this->state->get('list.ordering'));
$listDirn	= $this->escape($this->state->get('list.direction'));
<# Dim hasPublished = JExt.GetActiveTaskCollection().Item("TaskParameters").ContainsKey("hasPublishedField") #>	
<# If hasPublished Then #>
$trashed	= $this->state->get('filter.published') == -2 ? true : false;
<# End If  #>
?>

<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&view=<#= StrConv(Value("Task.nameObjectList"), VbStrConv.Lowercase) #>'); ?>" method="post" name="adminForm" id="adminForm">
<?php if (!empty( $this->sidebar)) : ?>
	<div id="j-sidebar-container" class="span2">
		<?php echo $this->sidebar; ?>
	</div>
	<div id="j-main-container" class="span10">
<?php else : ?>
	<div id="j-main-container">
<?php endif;?>
		<?php
		// Search tools bar
		echo JLayoutHelper::render('joomla.searchtools.default', array('view' => $this));
		?>
		<?php if (empty($this->items)) : ?>
			<div class="alert alert-no-items">
				<?php echo JText::_('JGLOBAL_NO_MATCHING_RESULTS'); ?>
			</div>
		<?php else : ?>
			<table class="table table-striped" id="articleList">
				<thead>
					<tr>
						<th width="1%" class="center">
							<?php echo JHtml::_('grid.checkall'); ?>
						</th>
<#						Dim row
						Dim numCols as Integer = 1
			            For Each row In JComponentDataTable.Rows
							If row.ShowInList = True Then #>
						<th class="nowrap center hidden-phone">
							<?php echo JHtml::_('searchtools.sort', '<#=  row.ListLabel #>', 'a.<#=  row.Field #>', $listDirn, $listOrder); ?>
<#						numCols += 1 #>
						</th>
<#						    End If
			            Next #>
					</tr>
				</thead>
				<tfoot>
					<tr>
						<td colspan="<#= numCols #>">
							<?php echo $this->pagination->getListFooter(); ?>
						</td>
					</tr>
				</tfoot>
				<tbody>
					<?php foreach ($this->items as $i => $item) :
<#						    If StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
							$link = JRoute::_( 'index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&task=<#= Value("Task.nameObject") #>.edit&<#= Value("Task.primaryKey") #>='.$item-><#= Value("Task.primaryKey") #>);
							$canEdit	= $user->authorise('core.edit',			'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
							$canChange	= $user->authorise('core.edit.state',	'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
<#						    End If #>
						?>
						<tr class="row<?php echo $i % 2; ?>">
							<td class="center">
								<?php echo JHtml::_('grid.id', $i, $item-><#= Value("Task.primaryKey") #>); ?>
							</td>
<#				               For Each row In JComponentDataTable.Rows
								If row.ShowInList = True Then #>
							<td class="small hidden-phone">
<#								If row.Field = "published" Then #>
								<div class="btn-group">
									<?php echo JHtml::_('jgrid.published', $item->published, $i, '<#= StrConv(Value("Task.nameObjectList"), VbStrConv.Lowercase) #>.', $canChange, 'cb'); ?>
									<?php
									// Create dropdown items	
									$action = $trashed ? 'untrash' : 'trash';
									JHtml::_('actionsdropdown.' . $action, 'cb' . $i, '<#= StrConv(Value("Task.nameObjectList"), VbStrConv.Lowercase) #>');
	
									// Render dropdown list
									echo JHtml::_('actionsdropdown.render');
									?>
								</div>
<#								ElseIf row.Linkable = True AndAlso StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
								<?php if ($canEdit) : ?>
									<a href="<?php echo $link;?>">
									<?php echo $this->escape($item-><#= row.Field #>); ?>
									</a>
								<?php else : ?>
									<?php echo $this->escape($item-><#= row.Field #>); ?>
								<?php endif; ?>
<#								Else #>
								<?php echo $this->escape($item-><#= row.Field #>); ?>
<#								End If #>
							</td>
<#							   	End If
				               Next #>
						</tr>
					<?php endforeach; ?>
				</tbody>
			</table>
		<?php endif; ?>

		<input type="hidden" name="task" value="" />
		<input type="hidden" name="boxchecked" value="0" />
		<input type="hidden" id="list_fullordering" name="list[fullordering]" value="<?php echo $listOrder . ' ' . $listDirn;?>" />
		<?php echo JHtml::_('form.token'); ?>
	</div>
</form>