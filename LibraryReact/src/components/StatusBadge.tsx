interface Props {
  available: boolean
}

export default function StatusBadge({ available }: Props) {
  return (
    <span className={`px-2 py-0.5 rounded-full text-xs font-semibold ${
      available ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
    }`}>
      {available ? 'Available' : 'Checked Out'}
    </span>
  )
}
