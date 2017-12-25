<?php
/**
 * Listing entry point file for listing Component
 * 
 * @package    Listing
 * @subpackage com_listing
 * @license  GNU General Public License version 2 or later
 *
 */

// no direct access
defined('_JEXEC') or die('Restricted access');

// Get an instance of the controller prefixed by Listing
$controller = JControllerLegacy::getInstance('Listing');
// Perform the Request task
$controller->execute(JFactory::getApplication()->input->get('task'));
// Redirect if set by the controller
$controller->redirect();

?>
