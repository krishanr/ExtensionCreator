<?php
/**
 * Listing View for com_listing Component
 * 
 * @package    Listing
 * @subpackage com_listing
 * @license  GNU General Public License version 2 or later
 *
 *
 */

// No direct access
defined('_JEXEC') or die( 'Restricted access' );

/**
 * HTML View class for the Listing Component
 *
 * @package	Joomla.Components
 * @subpackage	Listing
 */
class ListingViewListing extends JViewLegacy
{
	function display($tpl = null)
	{
		$this->state 		= $this->get('State');
		$this->items 		= $this->get('Items');

		// Check for errors.
		if (count($errors = $this->get('Errors'))) {
			JError::raiseWarning(500, implode("\n", $errors));
			return false;
		}

		$this->params = &$this->state->params;

		parent::display($tpl);
	}
}
?>
