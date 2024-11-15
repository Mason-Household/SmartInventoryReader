import React from 'react';
import { IconButton, Tooltip } from '@mui/material';
import { HelpCircleIcon } from 'lucide-react';

const HelpOutline: React.FC = () => {
	return (
		<Tooltip title="Help">
			<IconButton>
				<HelpCircleIcon />
			</IconButton>
		</Tooltip>
	);
};

export default HelpOutline;