import React from 'react';
import { ScanRes } from '../Scanner/ScanResult';

interface HistoryProps {
	scanResults: ScanRes[];
}

const History: React.FC<HistoryProps> = ({ scanResults }) => {
	return (
		<div>
			<h2>Scan History</h2>
			<ul>
				{scanResults.map((result, index) => (
					<li key={index}>
						<p>Type: {result.type}</p>
						<p>Price: {result.suggested_price}</p>
						<p>Text: {result.text_found}</p>
					</li>
				))}
			</ul>
		</div>
	);
};

export default History;