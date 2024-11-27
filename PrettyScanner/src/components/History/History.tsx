import React from 'react';
import { ScanRes } from '../Scanner/ScanResult';
import { Box, List, ListItem, Typography } from '@mui/material';
import { Heading } from 'lucide-react';

interface HistoryProps {
	scanResults: ScanRes[];
}

const History: React.FC<HistoryProps> = ({ scanResults }) => {
	return (
		<Box>
			<Heading>Scan History</Heading>
			<List>
				{scanResults.map((result, index) => (
					<ListItem key={index}>
						<Typography>Type: {result.type}</Typography>
						<Typography>Price: {result.suggested_price}</Typography>
						<Typography>Text: {result.text_found}</Typography>
					</ListItem>
				))}
			</List>
		</Box>
	);
};

export default History;